﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;
using UnityEngine.Events;



namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// Base class for a guided missile.
    /// </summary>
    public class Missile : RigidbodyProjectile
    {
        [Header("Settings")]

        [SerializeField]
        protected float noLockLifetime = 4;

        [SerializeField]
        protected TargetProximityTriggerMode triggerMode = TargetProximityTriggerMode.OnDistanceIncrease;

        [SerializeField]
        protected float triggerDistance = 49;

        [SerializeField]
        protected float leadTargetThreshold = 1000;

        protected bool targetWasInsideTrigger = false;

        protected bool triggered = false;

        [Header("Guidance")]

        [SerializeField]
        protected PIDController3D steeringPIDController;

        [Header("Components")]

        [SerializeField]
        protected TargetLocker targetLocker;

        [SerializeField]
        protected VehicleEngines3D engines;   

        protected bool locked = false;

        public UnityEvent onTargetLocked;
        public UnityEvent onTargetLockLost;

        private bool isTrackingEnabled = false;

        public override float Speed
        {
            get { return engines.GetDefaultMaxSpeedByAxis(false).z; }
        }

        public override float Range
        {
            get { return targetLocker.LockingRange; }
        }

        public override float Damage(HealthType healthType)
        {
            for (int i = 0; i < healthModifier.DamageOverrideValues.Count; ++i)
            {
                if (healthModifier.DamageOverrideValues[i].HealthType == healthType)
                {
                    return healthModifier.DamageOverrideValues[i].Value;
                }
            }

            return healthModifier.DefaultDamageValue;
        }



        protected override void Reset()
        {

            base.Reset();

            m_Rigidbody.useGravity = false;
            m_Rigidbody.mass = 1;
            m_Rigidbody.drag = 3;
            m_Rigidbody.angularDrag = 4;

            // Add/get engines
            engines = transform.GetComponentInChildren<VehicleEngines3D>();
            if (engines == null)
            {
                engines = gameObject.AddComponent<VehicleEngines3D>();
            }

            // Add/get target locker
            targetLocker = transform.GetComponentInChildren<TargetLocker>();
            if (targetLocker == null)
            {
                targetLocker = gameObject.AddComponent<TargetLocker>();
            }

            detonator.DetonatingDuration = 2;

            disableAfterDistanceCovered = false;

            areaEffect = true;

            healthModifier.DefaultDamageValue = 1000;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            targetWasInsideTrigger = false;
            triggered = false;
        }


        protected override void Awake()
        {
            base.Awake();

            if (collisionScanner != null)
            {
                collisionScanner.Rigidbody = m_Rigidbody;
            }
        }


        /// <summary>
        /// Set the target.
        /// </summary>
        /// <param name="target">The new target.</param>
        public virtual void SetTarget(Trackable target)
        {
            if (targetLocker != null)
            {
                targetLocker.SetTarget(target);
                SetLockState(LockState.Locked);
            }
        }

        public void EnableTracking()
        {
            isTrackingEnabled = true;
        }



        /// <summary>
        /// Set the lock state of the missile.
        /// </summary>
        /// <param name="lockState">The new lock state.</param>
        public virtual void SetLockState(LockState lockState)
        {
            if (targetLocker != null) targetLocker.SetLockState(lockState);

            if (lockState == LockState.Locked)
            {
                locked = true;
                onTargetLocked.Invoke();
            }
        }


        // Check if the trigger should be activated
        protected virtual void CheckTrigger()
        {
            if (triggered) return;

            if (targetLocker.Target == null) return;

            if (targetLocker.LockState != LockState.Locked) return;

            bool targetInsideTrigger = false;

            Collider[] colliders = Physics.OverlapSphere(transform.position, triggerDistance, collisionScanner.HitMask);
            for (int i = 0; i < colliders.Length; ++i)
            {
                if (colliders[i].transform.IsChildOf(targetLocker.Target.transform))
                {
                    targetInsideTrigger = true;

                    bool triggerNow = false;

                    switch (triggerMode)
                    {
                        case TargetProximityTriggerMode.OnTargetInRange:

                            triggerNow = true;
                            break;

                        case TargetProximityTriggerMode.OnDistanceIncrease:

                            float dist0 = GetClosestDistanceToTarget(0, colliders[i]);
                            float dist1 = GetClosestDistanceToTarget(Time.deltaTime, colliders[i]);

                            triggerNow = Mathf.Abs(dist1 - dist0) > 0.01f && dist1 > dist0;

                            Vector3 toTarget = targetLocker.Target.transform.position - transform.position;
                            Vector3 toTargetNext = (targetLocker.Target.transform.position + (targetLocker.Target.Rigidbody == null ? Vector3.zero : targetLocker.Target.Rigidbody.velocity * Time.deltaTime)) -
                                                    (transform.position + (m_Rigidbody == null ? Vector3.zero : m_Rigidbody.velocity * Time.deltaTime));

                            if (toTargetNext.magnitude < toTarget.magnitude) triggerNow = false;

                            break;
                    }

                    if (triggerNow)
                    {
                        triggered = true;
                        Detonate();
                        return;
                    }
                }
            }

            if (!targetInsideTrigger && targetWasInsideTrigger)
            {
                triggered = true;
                Detonate();
            }
            else
            {
                targetWasInsideTrigger = targetInsideTrigger;
            }
        }


        // Get the closest distance to the target based on a time projection (necessary because things can move very fast and the target can
        // change position a lot in one frame).
        protected virtual float GetClosestDistanceToTarget(float timeProjection, Collider targetCollider)
        {
            Vector3 targetOffset = targetLocker.Target.Rigidbody != null ? targetLocker.Target.Rigidbody.velocity * timeProjection : Vector3.zero;

            if (m_Rigidbody != null) targetOffset -= m_Rigidbody.velocity * timeProjection;

            Vector3 closestPoint = targetCollider.ClosestPoint(transform.position + targetOffset);

            return (closestPoint - transform.position).magnitude;
        }


        protected override void Update()
        {
            base.Update();

            CheckTrigger();

            if (isTrackingEnabled && targetLocker.LockState == LockState.Locked)
            {
                if (engines != null)
                {
                    // Get target velocity and position
                    Vector3 targetVelocity = targetLocker.Target.Rigidbody != null ? targetLocker.Target.Rigidbody.velocity : Vector3.zero;
                    Vector3 toTarget = targetLocker.Target.transform.position - transform.position;

                    Vector3 targetPos = targetLocker.Target.transform.position;

                    // Lead the target if within a certain threshold
                    if (toTarget.magnitude < leadTargetThreshold)
                    {
                        targetPos = TargetLeader.GetLeadPosition(transform.position, Speed, targetLocker.Target.transform.position, targetVelocity);
                    }

                    // Debugging: Log the positions and distance to target
                    Debug.Log($"Missile Position: {transform.position}, Target Position: {targetLocker.Target.transform.position}");
                    Debug.Log($"Distance to Target: {toTarget.magnitude}");


                    // Turn the missile towards the target position
                    Maneuvring.TurnToward(transform, targetPos, new Vector3(360, 360, 0), steeringPIDController);

                    // Log steering inputs for debugging
                    Vector3 steeringInput = steeringPIDController.GetControlValues();
                    Debug.Log($"Steering Input: {steeringInput}");

                    // Set engine inputs for missile movement
                    engines.SetSteeringInputs(steeringInput);
                    engines.SetMovementInputs(new Vector3(0, 0, 1));
                }
            }
            else
            {
                Debug.Log("Missile is not tracking the target.");

                // Detonate after lifetime
                if (locked)
                {
                    onTargetLockLost.Invoke();
                    detonator.Detonate(noLockLifetime);
                }

                // Clear steering inputs
                if (engines != null)
                {
                    engines.SetSteeringInputs(Vector3.zero);
                    engines.SetMovementInputs(new Vector3(0, 0, 1));
                }

            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Color c = Gizmos.color;

            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawWireSphere(transform.position, triggerDistance);

            Gizmos.color = c;

            if (targetLocker.Target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetLocker.Target.transform.position);
            }
        }

        protected override void MovementUpdate()
        {
            if (engines == null)
            {
                base.MovementUpdate();
            }
        }

        protected override void MovementFixedUpdate()
        {
            if (engines == null)
            {
                base.MovementFixedUpdate();
            }
        }

        public override void Detonate()
        {
            base.Detonate();
        }
    }
}