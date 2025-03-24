using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class provides an example control script for a space fighter.
    /// </summary>
    public class PlayerInput_InputSystem_EnterExitControls : VehicleInput
    {

        [Header("Enter/Exit Settings")]

        [SerializeField]
        protected GameAgent gameAgent;

        [SerializeField]
        protected bool prioritizeExiting = true;

        // The dependencies on the current vehicle
        protected VehicleEnterExitManager vehicleEnterExitManager;

        [SerializeField]
        protected bool setEnterExitPrompts = true;

        [SerializeField]
        protected string enterVehiclePrompt = "Press {control} to enter";

        [SerializeField]
        protected string exitVehiclePrompt = "Press {control} to exit";

        protected GeneralInputAsset input;


        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        protected override void Awake()
        {
            base.Awake();

            input = new GeneralInputAsset();

            // Steering
            input.GeneralControls.Use.performed += ctx => EnterExit();
        }


        protected virtual void Reset()
        {
            gameAgent = transform.root.GetComponentInChildren<GameAgent>();
        }

        /// <summary>
        /// Initialize this input script with a target object.
        /// </summary>
        /// <param name="inputTargetObject">The input target object.</param>
        /// <returns>Whether initialization succeeded</returns>
        protected override bool Initialize(GameObject inputTargetObject)
        {
            if (!base.Initialize(inputTargetObject)) return false;

            // Update the dependencies
            vehicleEnterExitManager = inputTargetObject.GetComponentInChildren<VehicleEnterExitManager>();
            if (vehicleEnterExitManager == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + vehicleEnterExitManager.GetType().Name + " component was not found on the vehicle.");
                }
                return false;
            }

            if (debugInitialization)
            {
                Debug.Log(GetType().Name + " successfully initialized.");
            }

            return true;

        }



        // Called every frame
        protected void EnterExit()
        {
            if (!CanRunInput()) return;

            if (setEnterExitPrompts)
            {
                vehicleEnterExitManager.SetPrompts(enterVehiclePrompt.Replace("{control}", GetControlDisplayString()),
                                                    exitVehiclePrompt.Replace("{control}", GetControlDisplayString()));
            }

            if (prioritizeExiting)
            {
                if (vehicleEnterExitManager.CanExitToChild())
                {
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
                else if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
                {
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
            }
            else
            {
                if (vehicleEnterExitManager.EnterableVehicles.Count > 0)
                {
                    // Check for input
                    Vehicle parent = vehicleEnterExitManager.EnterableVehicles[0].Vehicle;
                    vehicleEnterExitManager.EnterParent(0);
                    gameAgent.EnterVehicle(parent);
                }
                else if (vehicleEnterExitManager.CanExitToChild())
                {
                    // Check for input
                    Vehicle child = vehicleEnterExitManager.Child.Vehicle;
                    vehicleEnterExitManager.ExitToChild();
                    gameAgent.EnterVehicle(child);
                }
            }
        }


        protected virtual string GetControlDisplayString()
        {
            return input.GeneralControls.Use.GetBindingDisplayString();
        }


        protected override void OnInputUpdate()
        {
            if (setEnterExitPrompts)
            {
                vehicleEnterExitManager.SetPrompts(enterVehiclePrompt.Replace("{control}", GetControlDisplayString()),
                                                    exitVehiclePrompt.Replace("{control}", GetControlDisplayString()));
            }
        }
    }
}