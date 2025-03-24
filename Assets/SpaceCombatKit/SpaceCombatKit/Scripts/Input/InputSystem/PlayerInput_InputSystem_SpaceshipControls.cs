using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VSX.Controls;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Input script for controlling the steering and movement of a space fighter vehicle.
    /// </summary>
    public class PlayerInput_InputSystem_SpaceshipControls : VehicleInput
    {

        protected SCKInputAsset input;


        [Header("Control Scheme")]

        [Tooltip("Whether the vehicle should yaw when rolling.")]
        [SerializeField]
        protected bool linkYawAndRoll = false;

        [Tooltip("How much the vehicle should yaw when rolling.")]
        [SerializeField]
        protected float yawRollRatio = 1;


        [Header("Auto Roll")]

        protected InputDevice lastSteeringInputDevice;

        [System.Serializable]
        public class AutoRollActivation
        {
            public bool mouseKeyboard;
            public bool gamepad;
            public bool joystick;
        }

        [Tooltip("Whether auto roll should be activated - based on the last steering input device.")]
        [SerializeField]
        protected AutoRollActivation autoRollActivation;
        protected bool lastRollWasAutoRoll;

        [Tooltip("The amount of roll that is added proportional to the bank angle.")]
        [SerializeField]
        protected float autoRollStrength = 0.04f;

        [Tooltip("The maximum roll input value that auto roll can set.")]
        [SerializeField]
        protected float maxAutoRoll = 0.2f;

        [Tooltip("The time after the last roll input when auto roll will kick in.")]
        [SerializeField]
        protected float autoRollDelay = 0.5f;

        protected float lastRollTime;


        [Header("Mouse Steering")]

        [SerializeField]
        protected bool mouseEnabled = true;
        public bool MouseEnabled
        {
            get { return mouseEnabled; }
            set { mouseEnabled = value; }
        }

        [SerializeField]
        protected MouseSteeringType mouseSteeringType;
        public MouseSteeringType MouseSteeringType
        {
            get { return mouseSteeringType; }
            set { mouseSteeringType = value; }
        }

        [Tooltip("Invert the vertical mouse input.")]
        [SerializeField]
        protected bool mouseVerticalInverted = false;

        [Tooltip("Invert the horizontal mouse input.")]
        [SerializeField]
        protected bool mouseHorizontalInverted = false;


        [Tooltip("Whether this script controls the HUD cursor.")]
        [SerializeField]
        protected bool controlHUDCursor = true;


        [Header("Mouse Screen Position Settings")]

        [Tooltip("The fraction of the viewport (based on the screen width) around the screen center inside which the mouse position does not affect the ship steering.")]
        [SerializeField]
        protected float mouseDeadRadius = 0.1f;

        [Tooltip("How far the mouse reticule is allowed to get from the screen center.")]
        [SerializeField]
        protected float maxReticleDistanceFromCenter = 0.475f;

        [SerializeField]
        protected float reticleMovementSpeed = 1;

        [Tooltip("How much the ship pitches (local X axis rotation) based on the mouse distance from screen center.")]
        [SerializeField]
        protected AnimationCurve mousePositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        protected bool centerCursorOnInputEnabled = true;


        [Header("Mouse Delta Position Settings")]

        [SerializeField]
        protected float mouseDeltaPositionSensitivity = 0.05f;

        [SerializeField]
        protected AnimationCurve mouseDeltaPositionInputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Keyboard Steering")]

        [Tooltip("Invert the pitch (local X rotation) input.")]
        [SerializeField]
        protected bool nonMouseVerticalInverted = false;

        [Tooltip("Invert the pitch (local X rotation) input.")]
        [SerializeField]
        protected bool nonMouseHorizontalInverted = false;


        [Header("Throttle")]

        [SerializeField]
        protected bool setThrottle = false;
        protected float throttle = 0;

        [SerializeField]
        protected float throttleSensitivity = 1;

        protected bool steeringEnabled = true;

        protected bool movementEnabled = true;


        [Header("Boost")]

        [SerializeField]
        protected float boostChangeSpeed = 3;
        protected Vector3 boostTarget = Vector3.zero;

        // Reference to the engines component on the current vehicle
        protected VehicleEngines3D spaceVehicleEngines;

        protected HUDCursor hudCursor;
        protected Vector3 reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);


        protected override void Awake()
        {
            base.Awake();

            input = new SCKInputAsset();
            
            InputSystem.onDeviceChange += OnDeviceChange;

            // Steering
            input.SpacefighterControls.Steer.performed += ctx => Steer(ctx.ReadValue<Vector2>());

            // Strafing
            input.SpacefighterControls.Strafe.performed += ctx => Strafe(ctx.ReadValue<Vector2>());

            // Roll
            input.SpacefighterControls.Roll.performed += ctx => Roll(ctx.ReadValue<float>());

            // Acceleration
            input.SpacefighterControls.Throttle.performed += ctx => Forward(ctx.ReadValue<float>());

            // Boost
            input.SpacefighterControls.Boost.performed += ctx => Boost(ctx.ReadValue<float>());

        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        
        protected override bool Initialize(Vehicle vehicle)
        {

            reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);

            if (!base.Initialize(vehicle)) return false;

            // Clear dependencies
            spaceVehicleEngines = null;

            // Make sure the vehicle has a space vehicle engines component
            spaceVehicleEngines = vehicle.GetComponent<VehicleEngines3D>();

            hudCursor = vehicle.GetComponentInChildren<HUDCursor>();

            if (spaceVehicleEngines == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + spaceVehicleEngines.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " component successfully initialized.");
            }

            if (centerCursorOnInputEnabled && controlHUDCursor && hudCursor != null)
            {
                hudCursor.CenterCursor();
            }

            return true;
        }


        /// <summary>
        /// Enable steering input.
        /// </summary>
        public virtual void EnableSteering()
        {
            steeringEnabled = true;
        }


        /// <summary>
        /// Disable steering input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current steering values.</param>
        public virtual void DisableSteering(bool clearCurrentValues)
        {
            steeringEnabled = false;

            if (clearCurrentValues)
            {
                spaceVehicleEngines.SetSteeringInputs(Vector3.zero);
            }
        }


        /// <summary>
        /// Enable movement input.
        /// </summary>
        public virtual void EnableMovement()
        {
            movementEnabled = true;
        }


        /// <summary>
        /// Disable the movement input.
        /// </summary>
        /// <param name="clearCurrentValues">Whether to clear current throttle values.</param>
        public virtual void DisableMovement(bool clearCurrentValues)
        {
            movementEnabled = false;

            if (clearCurrentValues)
            {
                spaceVehicleEngines.SetMovementInputs(Vector3.zero);

                spaceVehicleEngines.SetBoostInputs(Vector3.zero);
            }
        }


        protected virtual void Steer(Vector2 steer)
        {
            if (!CanRunInput() || !steeringEnabled) return;

            lastSteeringInputDevice = input.SpacefighterControls.Steer.activeControl.device;
            if (lastSteeringInputDevice is Mouse)
            {
                MouseSteering(steer);
            }
            else if (lastSteeringInputDevice is Gamepad)
            {
                GamepadSteering(steer);
            }
        }


        protected virtual void MouseSteering(Vector2 steer)
        {
            if (!mouseEnabled) return;

            if (mouseSteeringType == MouseSteeringType.ScreenPosition)
            {
                if (hudCursor != null)
                {
                    Vector3 delta = new Vector3(steer.x / Screen.width, steer.y / Screen.height, 0);

                    if (mouseVerticalInverted) delta.y *= -1;
                    if (mouseHorizontalInverted) delta.x *= -1;

                    // Add the delta 
                    reticleViewportPosition += delta * reticleMovementSpeed;

                    // Center it
                    Vector3 centeredReticleViewportPosition = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

                    // Prevent distortion before clamping
                    centeredReticleViewportPosition.x *= (float)Screen.width / Screen.height;

                    // Clamp
                    centeredReticleViewportPosition = Vector3.ClampMagnitude(centeredReticleViewportPosition, maxReticleDistanceFromCenter);

                    // Convert back to proper viewport
                    centeredReticleViewportPosition.x /= (float)Screen.width / Screen.height;

                    reticleViewportPosition = centeredReticleViewportPosition + new Vector3(0.5f, 0.5f, 0);
                }
                else
                {
                    reticleViewportPosition = Mouse.current.position.ReadValue();
                }
            }
            else if (mouseSteeringType == MouseSteeringType.DeltaPosition)
            {
                reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);
            }

            if (controlHUDCursor && hudCursor != null)
            {
                hudCursor.SetViewportPosition(reticleViewportPosition);
            }

            // Implement control type
            Vector3 processedScreenInputs = Vector3.zero;
            if (mouseSteeringType == MouseSteeringType.ScreenPosition)
            {
                processedScreenInputs = reticleViewportPosition - new Vector3(0.5f, 0.5f, 0);

                processedScreenInputs.x *= (float)Screen.width / Screen.height;

                float amount = Mathf.Max(processedScreenInputs.magnitude - mouseDeadRadius, 0) / (maxReticleDistanceFromCenter - mouseDeadRadius);

                processedScreenInputs.x /= (float)Screen.width / Screen.height;

                processedScreenInputs = mousePositionInputCurve.Evaluate(amount) * processedScreenInputs.normalized;


            }
            else if (mouseSteeringType == MouseSteeringType.DeltaPosition)
            {
                processedScreenInputs = mouseDeltaPositionSensitivity * steer;
                processedScreenInputs = Mathf.Clamp(mouseDeltaPositionInputCurve.Evaluate(processedScreenInputs.magnitude), 0, 1) * processedScreenInputs.normalized;
            }

            Vector3 steerInputs = Vector3.zero;
            steerInputs.x = -processedScreenInputs.y;
            steerInputs.y = processedScreenInputs.x;


            if (mouseSteeringType != MouseSteeringType.ScreenPosition)
            {
                steerInputs.x *= (mouseVerticalInverted ? -1 : 1);
                steerInputs.y *= (mouseHorizontalInverted ? -1 : 1);
            }
           
            steerInputs.x = Mathf.Clamp(steerInputs.x, -1f, 1f);

            steerInputs.y = Mathf.Clamp(steerInputs.y, -1f, 1f);

            // Linked yaw and roll
            if (linkYawAndRoll && mouseSteeringType != MouseSteeringType.ScreenPosition)
            {
                steerInputs.z = Mathf.Clamp(-steerInputs.y * yawRollRatio, -1f, 1f);
            }
            else
            {
                steerInputs.z = spaceVehicleEngines.SteeringInputs.z;
            }

            spaceVehicleEngines.SetSteeringInputs(steerInputs);
        }


        protected virtual void GamepadSteering(Vector2 steer)
        {
            reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);

            if (controlHUDCursor && hudCursor != null)
            {
                hudCursor.SetViewportPosition(reticleViewportPosition);
            }

            Vector3 steerInputs = Vector3.zero;
            steerInputs.x = -steer.y;
            steerInputs.y = steer.x;

            steerInputs.x *= (nonMouseVerticalInverted ? -1 : 1);
            steerInputs.y *= (nonMouseHorizontalInverted ? -1 : 1);

            // Linked yaw and roll
            if (linkYawAndRoll)
            {
                steerInputs.z = Mathf.Clamp(-steerInputs.y * yawRollRatio, -1f, 1f);
            }
            else
            {
                steerInputs.z = spaceVehicleEngines.SteeringInputs.z;
            }

            spaceVehicleEngines.SetSteeringInputs(steerInputs);
        }


        protected virtual void Strafe(Vector2 strafe)
        {
            if (!CanRunInput() || !movementEnabled) return;

            Vector3 movementInputs = spaceVehicleEngines.MovementInputs;

            movementInputs.x = strafe.x;
            movementInputs.y = strafe.y;

            spaceVehicleEngines.SetMovementInputs(movementInputs);
        }


        protected virtual void Forward(float throttle)
        {
            if (!CanRunInput() || !movementEnabled) return;

            Vector3 movementInputs = spaceVehicleEngines.MovementInputs;

            if (setThrottle)
            {
                movementInputs.z = throttle;
            }
            else
            {
                movementInputs.z += throttle * throttleSensitivity * Time.deltaTime;
            }

            spaceVehicleEngines.SetMovementInputs(movementInputs);
        }


        protected virtual void Boost(float boost)
        {
            if (!CanRunInput() || !movementEnabled) return;

            boostTarget.z = boost;
            if (boostTarget.magnitude < 0.0001f) boostTarget = Vector3.zero;
        }


        protected virtual void Roll(float roll)
        {
            if (!CanRunInput() || !steeringEnabled) return;

            Vector3 steeringInputs = spaceVehicleEngines.SteeringInputs;
            steeringInputs.z = roll;
            spaceVehicleEngines.SetSteeringInputs(steeringInputs);
            lastRollWasAutoRoll = false;
        }


        protected virtual void AutoRoll()
        {
            bool cancelAutoRoll = false;

            if (lastSteeringInputDevice is Mouse && !autoRollActivation.mouseKeyboard) cancelAutoRoll = true;
            else if (lastSteeringInputDevice is Gamepad && !autoRollActivation.gamepad) cancelAutoRoll = true;
            else if (lastSteeringInputDevice is Joystick && !autoRollActivation.joystick) cancelAutoRoll = true;

            if (cancelAutoRoll)
            {
                if (lastRollWasAutoRoll)
                {
                    Vector3 _steeringInputs = spaceVehicleEngines.SteeringInputs;
                    _steeringInputs.z = 0;
                    spaceVehicleEngines.SetSteeringInputs(_steeringInputs);
                    lastRollWasAutoRoll = false;
                }

                return;
            }
           

            if (Mathf.Abs(input.SpacefighterControls.Roll.ReadValue<float>()) > 0.001f)
            {
                lastRollTime = Time.time;
            }

            if (Time.time - lastRollTime < autoRollDelay) return;

            // Project the forward vector down
            Vector3 flattenedFwd = spaceVehicleEngines.transform.forward;
            flattenedFwd.y = 0;
            flattenedFwd.Normalize();

            // Get the right
            Vector3 right = Vector3.Cross(Vector3.up, flattenedFwd);

            float angle = Vector3.Angle(right, spaceVehicleEngines.transform.right);

            if (Vector3.Dot(spaceVehicleEngines.transform.up, right) > 0)
            {
                angle *= -1;
            }

            Vector3 steeringInputs = spaceVehicleEngines.SteeringInputs;

            steeringInputs.z = Mathf.Clamp(angle * -1 * autoRollStrength, -1, 1);

            steeringInputs.z *= maxAutoRoll;

            steeringInputs.z *= 1 - Mathf.Abs(Vector3.Dot(spaceVehicleEngines.transform.forward, Vector3.up));

            spaceVehicleEngines.SetSteeringInputs(steeringInputs);

            lastRollWasAutoRoll = true;

        }


        protected override void OnInputUpdate()
        {
            AutoRoll();

            if (!setThrottle)
            {
                Vector3 movementInputs = spaceVehicleEngines.MovementInputs;
                movementInputs.z += input.SpacefighterControls.Throttle.ReadValue<float>() * throttleSensitivity * Time.deltaTime;
                spaceVehicleEngines.SetMovementInputs(movementInputs);
            }

            Vector3 boostInputs = spaceVehicleEngines.BoostInputs;

            boostInputs = Vector3.Lerp(boostInputs, boostTarget, boostChangeSpeed * Time.deltaTime);
            spaceVehicleEngines.SetBoostInputs(boostInputs);

            if (spaceVehicleEngines.BoostInputs.z > 0.5f)
            {
                Vector3 movementInputs = spaceVehicleEngines.MovementInputs;
                movementInputs.z = 1;
                spaceVehicleEngines.SetMovementInputs(movementInputs);
            }
        }


        protected virtual void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (Mouse.current == null || !mouseEnabled || Gamepad.current != null)
            {
                if (controlHUDCursor && hudCursor != null) hudCursor.CenterCursor();
                reticleViewportPosition = new Vector3(0.5f, 0.5f, 0);
            }
        }
    }
}