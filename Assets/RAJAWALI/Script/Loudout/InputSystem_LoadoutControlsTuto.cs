using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VSX.Controls;


namespace VSX.UniversalVehicleCombat.Loadout
{

    /// <summary>
    /// Example input system script for loadout controls.
    /// </summary>
    public class InputSystem_LoadoutControlsTuto : GeneralInput
    {

        [SerializeField]
        protected LoudoutUIControllerTuto loudoutUIControllerTuto;

        [Tooltip("The loadout camera controller.")]
        [SerializeField]
        protected LoadoutCameraControllerTuto loadoutCameraControllerTuto;

        protected LoadoutInputAsset input;

        protected Vector3 lookRotationInputs;
        protected bool lookRotationEngaged = false;

        [SerializeField]
        protected float cameraRotationSensitivity = 3f;


        protected override void Awake()
        {
            base.Awake();

            input = new LoadoutInputAsset();

            input.LoadoutControls.Menu.performed += ctx => Menu();

            input.LoadoutControls.Back.performed += ctx => Back();

            input.LoadoutControls.Start.performed += ctx => StartAction();

            //input.LoadoutControls.Accept.performed += ctx => Accept();

            input.LoadoutControls.Delete.performed += ctx => Delete();

            input.LoadoutControls.CycleVehicleSelection.performed += ctx => CycleVehicleSelection(ctx.ReadValue<float>());

            input.LoadoutControls.CycleModuleSelection.performed += ctx => CycleModuleSelection(ctx.ReadValue<float>());

            input.LoadoutControls.CycleModuleMountSelection.performed += ctx => CycleModuleMountSelection(ctx.ReadValue<float>());


        }


        protected virtual void OnEnable()
        {
            input.Enable();
        }


        protected virtual void OnDisable()
        {
            input.Disable();
        }


        protected virtual void Back()
        {
            if (loudoutUIControllerTuto.State == LoudoutUIControllerTuto.UIState.ModuleSelection)
            {
                loudoutUIControllerTuto.EnterVehicleSelection();
            }
            else
            {
                loudoutUIControllerTuto.MainMenu();
            }
        }

        public virtual void Menu()
        {
            loudoutUIControllerTuto.MainMenu();
        }

        public virtual void StartAction()
        {
            loudoutUIControllerTuto.StartMission(0);
        }

        /* public virtual void Accept()
         {
             if (loudoutUIControllerTuto.State == loudoutUIControllerTuto.UIState.VehicleSelection)
             {
                 loudoutUIControllerTuto.EnterModuleSelection();
             }
             else
             {
                 loudoutUIControllerTuto.EquipModule();
             }
         }*/


        protected virtual void CycleVehicleSelection(float val)
        {
            if (loudoutUIControllerTuto.State == LoudoutUIControllerTuto.UIState.VehicleSelection) loudoutUIControllerTuto.CycleVehicleSelection(val > 0 ? true : false);
        }

        protected virtual void CycleModuleSelection(float val)
        {
            if (loudoutUIControllerTuto.State == LoudoutUIControllerTuto.UIState.ModuleSelection) loudoutUIControllerTuto.CycleModuleSelection(val > 0 ? true : false);
        }

        protected virtual void CycleModuleMountSelection(float val)
        {
            if (loudoutUIControllerTuto.State == LoudoutUIControllerTuto.UIState.ModuleSelection) loudoutUIControllerTuto.CycleModuleMountSelection(val > 0 ? true : false);
        }

        protected virtual void Delete()
        {
            if (loudoutUIControllerTuto.State == LoudoutUIControllerTuto.UIState.ModuleSelection) loudoutUIControllerTuto.ClearSelectedModuleMount();
        }

        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();

            loadoutCameraControllerTuto.SetViewRotationInputs(input.LoadoutControls.Look.ReadValue<Vector2>() * cameraRotationSensitivity);

        }
    }
}
