using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// This class provides an example control script for a space fighter.
    /// </summary>
    public class SCKPlayerInput_InputSystem_ShipLanderControls : VehicleInput
    {
        [Header("Settings")]

        protected ShipLander shipLander;
        protected HUDShipLander hudShipLander;

        protected SCKInputAsset input;


        public bool overridePrompts = true;

        public string launchPrompt = "Press {control} to launch";
        public string landPrompt = "Press {control} to land";


        protected virtual void OnEnable()
        {
            input.Enable();
        }

        protected virtual void OnDisable()
        {
            input.Disable();
        }

        protected override void Awake()
        {
            base.Awake();

            input = new SCKInputAsset();

            // Steering
            input.SpacefighterControls.LaunchLand.performed += ctx => LaunchLand();
        }


        /// <summary>
        /// Initialize this input script with a target object.
        /// </summary>
        /// <param name="inputTargetObject">The input target object.</param>
        /// <returns>Whether initialization succeeded</returns>
        protected override bool Initialize(GameObject inputTargetObject)
        {
            if (!base.Initialize(inputTargetObject)) return false;

            shipLander = inputTargetObject.GetComponentInChildren<ShipLander>();

            hudShipLander = inputTargetObject.GetComponentInChildren<HUDShipLander>();

            if (shipLander == null)
            {
                if (debugInitialization)
                {
                    Debug.LogWarning(GetType().Name + " failed to initialize - the required " + shipLander.GetType().Name + " component was not found on the vehicle.");
                }

                return false;
            }
            else
            {
                if (overridePrompts)
                {
                    hudShipLander.SetPrompts(launchPrompt.Replace("{control}", GetControlDisplayString()),
                                            landPrompt.Replace("{control}", GetControlDisplayString()));
                }

                if (debugInitialization)
                {
                    Debug.Log(GetType().Name + " successfully initialized.");
                }

                return true;
            }
        }


        protected virtual string GetControlDisplayString()
        {
            return input.SpacefighterControls.LaunchLand.GetBindingDisplayString();
        }


        protected virtual void LaunchLand()
        {
            if (!CanRunInput()) return;

            switch (shipLander.CurrentState)
            {
                case (ShipLander.ShipLanderState.Launched):

                    shipLander.Land();

                    break;

                case (ShipLander.ShipLanderState.Landed):

                    shipLander.Launch();

                    break;
            }
        }
    }
}
