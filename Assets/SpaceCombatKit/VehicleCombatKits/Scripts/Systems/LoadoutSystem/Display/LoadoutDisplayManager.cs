using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Pooling;
using System.Reflection;

namespace VSX.UniversalVehicleCombat.Loadout
{
    /// <summary>
    /// This class manages the display of real vehicle and modules in the loadout menu.
    /// </summary>
	public class LoadoutDisplayManager : MonoBehaviour
    {

        [Tooltip("The loadout manager.")]
        [SerializeField]
        protected LoadoutManager loadoutManager;

        [Tooltip("The transform that display vehicles are parented to.")]
        [SerializeField]
        protected Transform vehicleHolder;

        // The list of added display vehicles
        protected List<Vehicle> displayVehicles = new List<Vehicle>();
        public virtual List<Vehicle> DisplayVehicles { get { return displayVehicles; } }

        protected List<Module> displayModules = new List<Module>();



        protected virtual void Reset()
        {
            loadoutManager = FindAnyObjectByType<LoadoutManager>();
            vehicleHolder = transform;
        }


        protected virtual void Awake()
        {
            //loadoutManager.onDataLoad.AddListener(AddDisplayVehicles);
         //   loadoutManager.onLoadoutChanged.AddListener(ShowVehicle);
        }

        protected virtual void Start()
        {
            //  Get LoadoutManager
            if (LoadoutManager.Instance != null)
            {
                loadoutManager = LoadoutManager.Instance;
            }
            else
            {
                loadoutManager = FindObjectOfType<LoadoutManager>();
            }

            if (loadoutManager == null)
            {
                Debug.LogError(" LoadoutManager NOT FOUND in DisplayManager!");
                return;
            }

            Debug.Log("DisplayManager connected to LoadoutManager");

            //  NOW register events
            loadoutManager.onDataLoad.AddListener(AddDisplayVehicles);
            loadoutManager.onLoadoutChanged.AddListener(ShowVehicle);

            // IMPORTANT: manually trigger once
            AddDisplayVehicles();
            ShowVehicle();
        }


        protected virtual void AddDisplayVehicles()
        {

            RemoveDisplayVehicles();

            if (loadoutManager.Items == null)
            {
                return;
            }

            List<LoadoutVehicleItem> vehicleItems = loadoutManager.Items.vehicles;

            foreach (LoadoutVehicleItem vehicleItem in vehicleItems)
            {
                Vehicle vehicle = GetDisplayVehicle(vehicleItem.vehiclePrefab);

                vehicle.gameObject.SetActive(false);
            }
        }


        protected virtual void RemoveModules(Vehicle vehicle)
        {
            foreach (ModuleMount moduleMount in vehicle.ModuleMounts)
            {
                moduleMount.RemoveAllModules();
            }

            for (int i = 0; i < displayModules.Count; ++i)
            {
                if (PoolManager.Instance != null)
                {
                    displayModules[i].gameObject.SetActive(false);
                }
                else
                {
                    Destroy(displayModules[i].gameObject);
                }

                displayModules.RemoveAt(i);
                i--;
            }
        }


        protected virtual void RemoveDisplayVehicles()
        {
            Vehicle[] displayVehiclesArray = displayVehicles.ToArray();

            foreach (Vehicle displayVehicle in displayVehiclesArray)
            {
                RemoveModules(displayVehicle);

                displayVehicles.Remove(displayVehicle);

                if (PoolManager.Instance != null)
                {
                    displayVehicle.transform.SetParent(null);
                    displayVehicle.gameObject.SetActive(false);
                }
                else
                {
                    Destroy(displayVehicle.gameObject);
                }
            }
        }


        protected virtual Vehicle GetDisplayVehicle(Vehicle vehiclePrefab)
        {
            Vehicle vehicle;

            if (PoolManager.Instance != null)
            {
                vehicle = PoolManager.Instance.Get(vehiclePrefab.gameObject, vehicleHolder).GetComponent<Vehicle>();
            }
            else
            {
                vehicle = Instantiate(vehiclePrefab, vehicleHolder.position, vehicleHolder.rotation, vehicleHolder);
            }

            // Make rigidbody kinematic

            Rigidbody r = vehicle.GetComponent<Rigidbody>();
            if (r != null) r.isKinematic = true;

            VehicleEngines3D engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines != null)
            {
                engines.ActivateEnginesAtStart = false;
                engines.SetEngineActivation(false);
            }

            // Prevent creation of default modules

            foreach (ModuleMount moduleMount in vehicle.ModuleMounts)
            {
                moduleMount.createDefaultModulesAtStart = false;
            }

            // Add to list

            displayVehicles.Add(vehicle);

            return vehicle;
        }


        protected virtual Module GetModule(Module modulePrefab)
        {
            if (PoolManager.Instance != null)
            {
                return PoolManager.Instance.Get(modulePrefab.gameObject, vehicleHolder).GetComponent<Module>();
            }
            else
            {
                Module module = Instantiate(modulePrefab, vehicleHolder.position, vehicleHolder.rotation);
                return module;
            }
        }


        protected virtual void ShowVehicle()
        {
            if (loadoutManager.Items == null) return;

            int vehicleIndex = loadoutManager.WorkingSlot.selectedVehicleIndex;

            // ALWAYS disable ALL vehicles first
            for (int i = 0; i < displayVehicles.Count; ++i)
            {
                displayVehicles[i].gameObject.SetActive(false);
            }

            // Safety check (only ONE needed)
            if (vehicleIndex < 0 || vehicleIndex >= displayVehicles.Count) return;

            // Activate ONLY selected vehicle
            Vehicle currentVehicle = displayVehicles[vehicleIndex];
            currentVehicle.gameObject.SetActive(true);

            List<LoadoutModuleItem> moduleItems = loadoutManager.Items.modules;

            // Remove old modules (prevent stacking)
            RemoveModules(currentVehicle);

            //  Add modules
            for (int i = 0; i < currentVehicle.ModuleMounts.Count; ++i)
            {
                if (loadoutManager.WorkingSlot.selectedModules.Count <= i) break;

                int moduleIndex = loadoutManager.WorkingSlot.selectedModules[i];

                if (moduleIndex != -1)
                {
                    if (currentVehicle.ModuleMounts[i].IsCompatible(moduleItems[moduleIndex].modulePrefab))
                    {
                        Module module = GetModule(moduleItems[moduleIndex].modulePrefab);
                        currentVehicle.ModuleMounts[i].AddModule(module, true);
                        displayModules.Add(module);
                    }
                    else
                    {
                        Debug.LogWarning(moduleItems[moduleIndex].modulePrefab.name + " not compatible with " +
                            currentVehicle.ModuleMounts[i].name + " on " + currentVehicle.name);
                    }
                }
                else
                {
                    currentVehicle.ModuleMounts[i].UnmountActiveModule();
                }
            }
        }
    }
}
