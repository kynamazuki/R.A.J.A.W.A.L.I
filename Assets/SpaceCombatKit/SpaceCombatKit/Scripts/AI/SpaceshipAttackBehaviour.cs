using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat.Radar;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{
    public class SpaceshipAttackBehaviour : AISpaceshipBehaviour
    {

        [Header("Primary Weapons")]

        [Tooltip("Whether to use primary weapons.")]
        [SerializeField]
        protected bool primaryWeaponsEnabled = true;

        [Tooltip("This is the minimum random amount of time the primary weapons will be firing continuously before a pause, when the target is in the sights.")]
        [SerializeField]
        protected float minPrimaryFiringPeriod = 1;

        [Tooltip("This is the maximum random amount of time the primary weapons will be firing continuously before a pause, when the target is in the sights.")]
        [SerializeField]
        protected float maxPrimaryFiringPeriod = 3;

        [Tooltip("This is the minimum random amount of time the primary weapons will pause before firing again, when the target is in the sights.")]
        [SerializeField]
        protected float minPrimaryFiringPause = 0.5f;

        [Tooltip("This is the maximum random amount of time the primary weapons will pause before firing again, when the target is in the sights.")]
        [SerializeField]
        protected float maxPrimaryFiringPause = 2;

        [Tooltip("This is the maximum angle to target (relative to the ship's forward vector) within which the AI will fire the primary weapons.")]
        [SerializeField]
        protected float maxFiringAngle = 15f;

        [Tooltip("This is the maximum distance to target within which the AI will fire the primary weapons.")]
        [SerializeField]
        protected float maxFiringDistance = 600;

        protected float primaryWeaponActionStartTime = 0;
        protected float primaryWeaponActionPeriod = 0f;
        protected bool primaryWeaponEngaged = false;
        protected bool primaryWeaponFiring = false;

        [Header("Accuracy Spread")]

        [Tooltip("Maximum aim spread in degrees.")]
        [SerializeField]
        protected float maxAimSpreadAngle = 2.5f;

        [Tooltip("Spread increases with distance.")]
        [SerializeField]
        protected bool distanceAffectsSpread = true;

        [Tooltip("Distance at which spread is maxed.")]
        [SerializeField]
        protected float spreadMaxDistance = 1000f;


        [Header("Secondary Weapons")]

        [Tooltip("Whether to use secondary weapons.")]
        [SerializeField]
        protected bool secondaryWeaponsEnabled = true;
        public virtual bool SecondaryWeaponsEnabled
        {
            get { return secondaryWeaponsEnabled; }
            set { secondaryWeaponsEnabled = value; }
        }

        [Tooltip("This is the minimum (x-value) and maximum (y-value) random interval between firing of the secondary weapons (missiles).")]
        [SerializeField]
        protected Vector2 minMaxSecondaryFiringInterval = new Vector2(3, 6);

        [Tooltip("If false, will fire a missile immediately upon engaging, otherwise, upon engaging a target, will wait sometime between 0 and 'Min Max Secondary Firing Interval' y-value to fire the first shot.")]
        [SerializeField]
        protected bool randomizeFirstSecondaryFiringTime = true;

        [SerializeField]
        protected Vector2 firstSecondaryFiringDelay = new Vector2(4f, 8f);

        protected bool hasFiredFirstMissile = false;


        protected float secondaryWeaponActionStartTime = 0;
        protected float secondaryWeaponActionPeriod = 0f;
        protected bool secondaryWeaponFiring = false;

        protected Weapons weapons;
        protected TriggerablesManager triggerablesManager;

        public UnityAction onSecondaryWeaponFired;
        

        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            weapons = vehicle.GetComponent<Weapons>();
            if (weapons == null) return false;
            
            triggerablesManager = vehicle.GetComponent<TriggerablesManager>();
            if (triggerablesManager == null) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines == null) return false;

            return true;

        }


        public override void StartBehaviour()
        {
            base.StartBehaviour();

            hasFiredFirstMissile = false;
            secondaryWeaponActionStartTime = Time.time;

            if (randomizeFirstSecondaryFiringTime)
            {
                secondaryWeaponActionPeriod = Random.Range(
                    firstSecondaryFiringDelay.x,
                    firstSecondaryFiringDelay.y
                );
            }
            else
            {
                secondaryWeaponActionPeriod = firstSecondaryFiringDelay.x;
            }
        }


        public override void StopBehaviour()
        {
            if (initialized) triggerablesManager.StopTriggeringAll();
        }
        

        protected virtual void StopPrimaryWeapon()
        {
            triggerablesManager.StartTriggeringAtIndex(0);
            primaryWeaponFiring = false;
        }

        

        protected virtual void SetPrimaryWeaponAction(bool fire)
        {
            if (fire)
            {
                triggerablesManager.StartTriggeringAtIndex(0);
                primaryWeaponFiring = true;

                primaryWeaponActionStartTime = Time.time;
                primaryWeaponActionPeriod = Random.Range(minPrimaryFiringPeriod, maxPrimaryFiringPeriod);
            }
            else
            {
                triggerablesManager.StopTriggeringAtIndex(0);
                primaryWeaponFiring = false;

                primaryWeaponActionStartTime = Time.time;
                primaryWeaponActionPeriod = Random.Range(minPrimaryFiringPause, maxPrimaryFiringPause);
            }
        }

        protected Vector3 ApplyAccuracySpread(Vector3 targetPosition)
        {
            Vector3 toTarget = targetPosition - vehicle.transform.position;
            float distance = toTarget.magnitude;

            float spreadFactor = 1f;

            if (distanceAffectsSpread)
                spreadFactor = Mathf.Clamp01(distance / spreadMaxDistance);

            float spreadAngle = maxAimSpreadAngle * spreadFactor;

            Quaternion spreadRotation = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            );

            return vehicle.transform.position + spreadRotation * toTarget;
        }


        /*protected virtual void PrimaryWeaponUpdate(Vector3 toTargetVector)
        {
            bool canFire = primaryWeaponsEnabled &&
                           Vector3.Angle(vehicle.transform.forward, toTargetVector) < maxFiringAngle &&
                           toTargetVector.magnitude < maxFiringDistance;

            if (canFire)
            {
                if (!primaryWeaponFiring)
                {
                    triggerablesManager.StartTriggeringAtIndex(0);
                    primaryWeaponFiring = true;
                }
            }
            else
            {
                if (primaryWeaponFiring)
                {
                    triggerablesManager.StopTriggeringAtIndex(0);
                    primaryWeaponFiring = false;
                }
            }
        }*/



        protected virtual void PrimaryWeaponUpdate(Vector3 toTargetVector)
        {
            // Do primary weapons
            bool canFire = primaryWeaponsEnabled && Vector3.Angle(vehicle.transform.forward, toTargetVector) < maxFiringAngle && toTargetVector.magnitude < maxFiringDistance;

            if (canFire)
            {
                if (!primaryWeaponEngaged)
                {
                    primaryWeaponEngaged = true;
                    SetPrimaryWeaponAction(true);
                }

                // Fire in bursts
                if (Time.time - primaryWeaponActionStartTime > primaryWeaponActionPeriod)
                {
                    SetPrimaryWeaponAction(!primaryWeaponFiring);
                }
            }
            else
            {
                if (primaryWeaponEngaged)
                {
                    primaryWeaponEngaged = false;
                    SetPrimaryWeaponAction(false);
                }
            }
        }


        protected void SecondaryWeaponUpdate()
        {
            if (!secondaryWeaponsEnabled || weapons.MissileWeapons.Count == 0)
                return;

            TargetLocker targetLocker = weapons.MissileWeapons[0].GetComponent<TargetLocker>();
            if (targetLocker == null || targetLocker.LockState != LockState.Locked)
                return;

            // Wait before first missile too
            if (Time.time - secondaryWeaponActionStartTime >= secondaryWeaponActionPeriod)
            {
                FireSecondaryWeapon();
            }
        }

        protected void FireSecondaryWeapon()
        {
            triggerablesManager.TriggerOnce(1);

            hasFiredFirstMissile = true;
            secondaryWeaponActionStartTime = Time.time;

            secondaryWeaponActionPeriod = Random.Range(
                minMaxSecondaryFiringInterval.x,
                minMaxSecondaryFiringInterval.y
            );

            onSecondaryWeaponFired?.Invoke();
        }



        /*protected virtual void SecondaryWeaponUpdate()
        {
            if (secondaryWeaponsEnabled && weapons.MissileWeapons.Count > 0)
            {
                TargetLocker targetLocker = weapons.MissileWeapons[0].GetComponent<TargetLocker>();
                if (targetLocker != null && targetLocker.LockState == LockState.Locked)
                {
                    if (Time.time - secondaryWeaponActionStartTime > secondaryWeaponActionPeriod)
                    {
                        triggerablesManager.TriggerOnce(1);
                        secondaryWeaponActionPeriod = Random.Range(minMaxSecondaryFiringInterval.x, minMaxSecondaryFiringInterval.y);
                        secondaryWeaponActionStartTime = Time.time;
                        if (onSecondaryWeaponFired != null) onSecondaryWeaponFired.Invoke();
                    }
                }
            }
        }*/


        /// <summary>
        /// Update the behaviour.
        /// </summary>
        public override bool BehaviourUpdate()
        {

            if (!base.BehaviourUpdate()) return false;

            if (weapons.WeaponsTargetSelector == null || weapons.WeaponsTargetSelector.SelectedTarget == null)
            {
                return false;
            }

            Vector3 velocity = weapons.WeaponsTargetSelector.SelectedTarget.Rigidbody == null ? Vector3.zero : weapons.WeaponsTargetSelector.SelectedTarget.Rigidbody.velocity;

            Vector3 leadTargetPos = weapons.GetAverageLeadTargetPosition(weapons.WeaponsTargetSelector.SelectedTarget.transform.position,velocity);

            Vector3 targetPos = ApplyAccuracySpread(leadTargetPos);


            Vector3 toTargetVector = targetPos - vehicle.transform.position;

            // Do primary weapons
            PrimaryWeaponUpdate(toTargetVector);

            // Do the secondary weapons
            SecondaryWeaponUpdate();
            
            // Turn toward target
            Maneuvring.TurnToward(vehicle.transform, targetPos, maxRotationAngles, shipPIDController.steeringPIDController);
            engines.SetSteeringInputs(shipPIDController.steeringPIDController.GetControlValues());

            Maneuvring.TranslateToward(engines.Rigidbody, targetPos, shipPIDController.movementPIDController);

            Vector3 movementInputs = shipPIDController.movementPIDController.GetControlValues();
            engines.SetMovementInputs(new Vector3(0, 0, movementInputs.z));

            return true;

        }
    }
}