using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat.Loadout
{
    /// <summary>
    /// This class manages the loadout menu scene.
    /// </summary>
    public class LoadoutManager : MonoBehaviour
    {
        public static LoadoutManager Instance;

        [Tooltip("The loadout items (vehicles and modules) to add when the scene starts.")]
        [SerializeField]
        protected LoadoutItems startingItems;
        protected LoadoutItems items;
        public virtual LoadoutItems Items
        {
            get { return items; }
        }

        [Tooltip("Whether a vehicle item is exclusive when assigned to a slot, i.e. it cannot be selected at another slot.")]
        [SerializeField]
        protected bool exclusiveVehicles = false;


        [Tooltip("Whether a module item is exclusive when assigned to a vehicle, i.e. it cannot be selected on another vehicle.")]
        [SerializeField]
        protected bool exclusiveModules = false;   // Whether modules are exclusive when assigned to vehicles


        [Tooltip("Whether to create one slot per vehicle item. This enables a loadout where different vehicle loadouts can be set up and saved but only one chosen. Will override 'Num Slots'.")]
        [SerializeField]
        protected bool slotPerVehicle = true;
        public bool SlotPerVehicle { get { return slotPerVehicle; } }

        [Tooltip("How many loadout slots to create.")]
        [SerializeField]
        protected int numSlots = 1;

        protected LoadoutSlot workingSlot;  // A slot that represents the current state of the loadout in terms of visualization, but data may not be finalized
        public virtual LoadoutSlot WorkingSlot { get { return workingSlot; } }

        [Tooltip("Whether to apply the vehicle selection as soon as it changes, i.e. if the player cycles through vehicles, they won't have to press any 'Select Vehicle' button to finalize their choice.")]
        [SerializeField]
        protected bool applyVehicleSelection = true;


        [Tooltip("Whether to apply the module selection as soon as it changes, i.e. if the player cycles through modules, they won't have to press any 'Select Module' button to finalize their choice.")]
        [SerializeField]
        protected bool applyModuleSelection = false;


        // Selection parameters

        protected int selectedModuleMountIndex = -1;
        public virtual int SelectedModuleMountIndex { get { return selectedModuleMountIndex; } }


        protected List<int> selectableVehicleIndexes = new List<int>();
        public virtual List<int> SelectableVehicleIndexes { get { return selectableVehicleIndexes; } }  // Indexes refer to the current LoadoutItems vehicle and module lists


        protected List<int> selectableModuleIndexes = new List<int>();
        public virtual List<int> SelectableModuleIndexes { get { return selectableModuleIndexes; } }    // Indexes refer to the current LoadoutItems vehicle and module lists

        [Tooltip("The component that handles loadout data saving. Can be changed at runtime.")]
        [SerializeField]
        public LoadoutDataManager loadoutDataManager;
        public LoadoutDataManager LoadoutDataManager
        {
            get { return loadoutDataManager; }
            set { loadoutDataManager = value; }
        }

        [Tooltip("Whether to load the last selected slot or just start at the first slot after loading.")]
        [SerializeField]
        protected bool loadSelectedSlot = true;

        [Tooltip("Called when the loadout data is created or loaded.")]
        public UnityEvent onDataLoad;

        [Tooltip("Called when something on the loadout changes, so that UI and other components can update.")]
        public UnityEvent onLoadoutChanged;

        protected LoadoutData loadoutData;
        public LoadoutData LoadoutData { get { return loadoutData; } }

        public bool isNewGameStart = true;
        [SerializeField] public bool useVehicleUnlockSystem = true;
        [Header("Arcade Visible Fighters Only")]
        [SerializeField]
        private List<int> arcadeVisibleVehicleIndexes = new List<int>() { 0, 4, 8, 12 };

        private int arcadeCurrentDisplayIndex = 0;

        [Header("Fighter Category System")]
        public List<FighterCategory> fighterCategories = new List<FighterCategory>();

        private int selectedCategoryIndex = -1;
        public int SelectedCategoryIndex => selectedCategoryIndex;

        private List<int> selectableVariantIndexes = new List<int>();
        public List<int> SelectableVariantIndexes => selectableVariantIndexes;

        private int selectedVariantLocalIndex = 0;
        public int SelectedVariantLocalIndex => selectedVariantLocalIndex;

        public static bool EnterArcadeFromMainMenu = false;

        protected virtual void Reset()
        {
            loadoutDataManager = FindObjectOfType<LoadoutDataManager>();
        }



        protected virtual void Awake()
        {
            loadoutData = new LoadoutData();
            InitializeWorkingSlot();
            Instance = this;
        }


        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }


        protected virtual void Start()
        {
            if (items == null) SetItems(startingItems);

            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.ResetMission();
            }

            if (loadoutDataManager == null)
            {
                loadoutDataManager = FindObjectOfType<LoadoutDataManagerJSON>();
                if (loadoutDataManager == null)
                {
                    Debug.LogError("No LoadoutDataManager found in scene.");
                }
                else
                {
                    Debug.Log("LoadoutDataManager assigned at runtime.");
                }
            }
        }


        // Initialize the working slot
        protected virtual void InitializeWorkingSlot()
        {
            workingSlot = new LoadoutSlot();
            workingSlot.selectedVehicleIndex = -1;
        }


        // Called when the loadout items change
        public virtual void OnDataLoad()
        {
            onDataLoad.Invoke();

        }


        // Called when something on the loadout changes, e.g. a vehicle or module selection occurs
        public virtual void OnLoadoutChanged()
        {
            onLoadoutChanged.Invoke();
        }


        /// <summary>
        /// Set the loadout items.
        /// </summary>
        /// <param name="items">The new items.</param>
        public virtual void SetItems(LoadoutItems items, bool loadPersistentData = true)
        {

            this.items = items;

            if (loadPersistentData)
            {
                LoadPersistentData();
            }

            if (loadoutData.Slots.Count == 0)
            {
                LoadDefaultData();
            }
        }


        // Slots


        // Initialize/build the slots from the loadout data
        protected virtual void LoadDefaultData()
        {

            // Create the slots
            loadoutData.Slots = new List<LoadoutSlot>();

            numSlots = (slotPerVehicle && items != null) ? items.vehicles.Count : Mathf.Max(numSlots, 1);
            for (int i = 0; i < numSlots; ++i)
            {
                LoadoutSlot newSlot = new LoadoutSlot();
                newSlot.selectedVehicleIndex = -1;
                loadoutData.Slots.Add(newSlot);
            }

            if (items == null) return;



            // Fill the slots with data

            if (items != null)
            {
                for (int i = 0; i < loadoutData.Slots.Count; ++i)
                {
                    if (useVehicleUnlockSystem)
                    {
                        int progress = loadoutData.currentMissionIndex;

                        if (i < items.vehicles.Count)
                        {
                            loadoutData.Slots[i].selectedVehicleIndex = i; // ALWAYS assign
                        }
                        else
                        {
                            loadoutData.Slots[i].selectedVehicleIndex = -1;
                        }
                    }
                    else
                    {
                        // No lock system → allow all
                        loadoutData.Slots[i].selectedVehicleIndex = i < items.vehicles.Count ? i : -1;
                    }

                    // 🚨 KEEP THIS PART (modules setup)
                    if (loadoutData.Slots[i].selectedVehicleIndex != -1)
                    {
                        List<int> defaultModules = GetDefaultModules(loadoutData.Slots[i].selectedVehicleIndex);

                        foreach (int defaultModule in defaultModules)
                        {
                            bool used = false;
                            if (exclusiveModules)
                            {
                                foreach (LoadoutSlot slot in loadoutData.Slots)
                                {
                                    foreach (int usedModuleIndex in slot.selectedModules)
                                    {
                                        if (usedModuleIndex == defaultModule)
                                        {
                                            used = true;
                                            break;
                                        }
                                    }
                                    if (used) break;
                                }
                            }

                            if (!used)
                            {
                                loadoutData.Slots[i].selectedModules.Add(defaultModule);
                            }
                            else
                            {
                                loadoutData.Slots[i].selectedModules.Add(-1);
                            }
                        }
                    }
                }
            }

            OnDataLoad();

            loadoutData.selectedSlotIndex = -1;
            if (loadoutData.Slots.Count > 0) SelectSlot(0);

        }

        



        /// <summary>
        /// Select a loadout slot.
        /// </summary>
        /// <param name="slotIndex">The index of the slot.</param>
        public virtual void SelectSlot(int slotIndex)
        {
            slotIndex = Mathf.Clamp(slotIndex, -1, loadoutData.Slots.Count - 1);

            loadoutData.selectedSlotIndex = slotIndex;

            RevertWorkingToActiveSlot();

            UpdateSelectableVehicles();

            SelectModuleMount(0);

            OnLoadoutChanged();

        }


        /// <summary>
        /// Cycle through the loadout slots forward or back.
        /// </summary>
        /// <param name="forward">Whether to cycle forward (back if false)</param>
        /// <param name="wrap">Whether to wrap around to beginning when cycling past the end, or wrap to the end when cycling back past the beginning.</param>
        public virtual void CycleSlot(bool forward, bool wrap = false)
        {
            // Cycle forward or back

            int index = loadoutData.selectedSlotIndex;
            if (forward)
            {
                index++;
            }
            else
            {
                index--;
            }

            // Wrap

            if (wrap)
            {
                if (index < 0)
                {
                    index = loadoutData.Slots.Count - 1;
                }
                else if (index >= loadoutData.Slots.Count)
                {
                    index = Mathf.Min(0, loadoutData.Slots.Count - 1);
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, loadoutData.Slots.Count - 1);
            }

            // Select the new slot
            SelectSlot(index);
        }



        /// <summary>
        /// Clear the selected slot.
        /// </summary>
        public virtual void ClearSelectedSlot()
        {
            if (loadoutData.SelectedSlot != null && loadoutData.SelectedSlot.selectedVehicleIndex != -1)
            {
                loadoutData.SelectedSlot.selectedVehicleIndex = -1;
                loadoutData.SelectedSlot.selectedModules.Clear();
                RevertWorkingToActiveSlot();

                OnLoadoutChanged();
            }
        }

        protected virtual List<int> GetDefaultModules(int vehicleIndex)
        {

            List<int> defaultModuleIndexes = new List<int>();
            LoadoutVehicleItem vehicleItem = items.vehicles[vehicleIndex];
            int numModuleMounts = vehicleItem.vehiclePrefab.ModuleMounts.Count;

            for (int i = 0; i < numModuleMounts; ++i)
            {
                defaultModuleIndexes.Add(-1);
            }

            if (vehicleItem.predefinedModules != null && vehicleItem.predefinedModules.Count > 0)
            {

                foreach (PredefinedModuleAssignment assignment in vehicleItem.predefinedModules)
                {
                    if (assignment.moduleMountIndex >= 0 && assignment.moduleMountIndex < numModuleMounts)
                    {
                        int index = items.modules.IndexOf(assignment.moduleItem);

                        if (index != -1)
                        {
                            defaultModuleIndexes[assignment.moduleMountIndex] = index;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Falling back to defaultLoadout...");
                // fallback logic
            }

            return defaultModuleIndexes;
        }





        // Vehicle selection


        /// <summary>
        /// Select a vehicle in the loadout menu.
        /// </summary>
        /// <param name="index">The index of the vehicle to select.</param>
        public virtual void SelectVehicle(int vehicleIndex)
        {
            if (useVehicleUnlockSystem)
            {
                int progress = loadoutData.currentMissionIndex;
                int arcadeTier = GetArcadeTierFromVehicleIndex(vehicleIndex);

                if (arcadeTier > progress)
                {
                    Debug.Log("Vehicle Locked!");
                    return;
                }
            }

            if (vehicleIndex < 0 || vehicleIndex >= items.vehicles.Count) return;
            if (vehicleIndex == workingSlot.selectedVehicleIndex) return;

            workingSlot.selectedVehicleIndex = vehicleIndex;

            // Update the working slot

            if (vehicleIndex == loadoutData.SelectedSlot.selectedVehicleIndex)
            {

                RevertWorkingToActiveSlot();
            }
            else
            {
                // Initialize the module selection

                List<int> defaultModules = GetDefaultModules(workingSlot.selectedVehicleIndex);

                workingSlot.selectedModules.Clear();

                foreach (int defaultModule in defaultModules)
                {
                    bool used = false;
                    if (exclusiveModules)
                    {
                        foreach (LoadoutSlot slot in loadoutData.Slots)
                        {
                            foreach (int usedModuleIndex in slot.selectedModules)
                            {
                                if (usedModuleIndex == defaultModule)
                                {
                                    used = true;
                                    break;
                                }
                            }
                            if (used) break;
                        }
                    }

                    if (!used)
                    {
                        workingSlot.selectedModules.Add(defaultModule);
                    }
                    else
                    {
                        workingSlot.selectedModules.Add(-1);
                    }
                }
            }

            if (applyVehicleSelection) SaveWorkingToActiveSlot();

            SelectModuleMount(0);
            
            OnLoadoutChanged();

            

        }


        /// <summary>
        /// Cycle through the vehicles forward or back.
        /// </summary>
        /// <param name="forward">Whether to cycle forward (back if false)</param>
        /// <param name="wrap">Whether to wrap around to beginning when cycling past the end, or wrap to the end when cycling back past the beginning.</param>
       /* public virtual void CycleVehicleSelection(bool forward, bool wrap = false)
        {

            if (selectableVehicleIndexes.Count == 0) return;

            // Cycle up or down
            int index = selectableVehicleIndexes.IndexOf(workingSlot.selectedVehicleIndex);

            if (forward)
            {
                index++;
            }
            else
            {
                index--;
            }

            // Wrap

            if (wrap)
            {
                if (index < 0)
                {
                    index = selectableVehicleIndexes.Count - 1;
                }
                else if (index >= selectableVehicleIndexes.Count)
                {
                    index = Mathf.Min(0, selectableVehicleIndexes.Count - 1);
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, selectableVehicleIndexes.Count - 1);
            }

            // Select the new vehicle
            if (index != -1) SelectVehicle(selectableVehicleIndexes[index]);

        }*/


        public virtual LoadoutVehicleItem GetSelectedVehicleItem()
        {
            if (loadoutData == null) return null;
            if (loadoutData.SelectedSlot == null) return null;

            int index = loadoutData.SelectedSlot.selectedVehicleIndex;

            if (index == -1) return null;

            // LOCK CHECK
            if (useVehicleUnlockSystem)
            {
                int progress = loadoutData.currentMissionIndex;

                if (index > progress)
                {
                    Debug.Log("Using locked vehicle blocked!");
                    return null;
                }
            }

            return items.vehicles[index];
        }


        // Update the list of vehicles that can be selected at the current slot
        public virtual void UpdateSelectableVehicles()
        {

            selectableVehicleIndexes.Clear();

            if (items == null) return;

            for (int i = 0; i < items.vehicles.Count; ++i)
            {
                bool used = false;

                if (exclusiveVehicles)
                {
                    foreach (LoadoutSlot slot in loadoutData.Slots)
                    {
                        if (loadoutData.Slots.IndexOf(slot) != loadoutData.selectedSlotIndex && slot.selectedVehicleIndex == i)
                        {
                            used = true;
                            break;
                        }
                    }
                }
                //if (!used) selectableVehicleIndexes.Add(i);

                if (useVehicleUnlockSystem)
                {
                    int progress = loadoutData.currentMissionIndex;

                    int arcadeTier = GetArcadeTierFromVehicleIndex(i);

                    bool unlocked = arcadeTier != -1 && arcadeTier <= progress;

                    if (!used && unlocked)
                    {
                        selectableVehicleIndexes.Add(i);
                    }
                }
                else
                {
                    if (!used)
                    {
                        selectableVehicleIndexes.Add(i);
                    }
                }
            }



        }


        // Module mount selection

        /// <summary>
        /// Select the first module mount on the vehicle.
        /// </summary>
        public virtual void SelectFirstModuleMount()
        {
            SelectModuleMount(0);
        }


        /// <summary>
        /// Select a module mount on the selected vehicle.
        /// </summary>
        /// <param name="moduleMountIndex">The module mount index in the vehicle's Module Mounts list.</param>
        public virtual void SelectModuleMount(int moduleMountIndex)
        {
            selectedModuleMountIndex = Mathf.Min(moduleMountIndex, workingSlot.selectedModules.Count - 1);

            UpdateSelectableModules();

            RevertWorkingToActiveSlot();

            OnLoadoutChanged();

        }


        /// <summary>
        /// Cycle forward or back through the module mounts on the selected vehicle.
        /// </summary>
        /// <param name="forward">Whether to cycle forward (back if false)</param>
        /// <param name="wrap">Whether to wrap around to beginning when cycling past the end, or wrap to the end when cycling back past the beginning.</param>
        public virtual void CycleModuleMount(bool forward, bool wrap = false)
        {

            if (items.vehicles[workingSlot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count == 0) return;

            // Cycle forward or back

            int index = selectedModuleMountIndex;
            if (forward)
            {
                index++;
            }
            else
            {
                index--;
            }

            // Wrap

            if (wrap)
            {
                if (index < 0)
                {
                    index = items.vehicles[workingSlot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count - 1;
                }
                else if (index >= items.vehicles[workingSlot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count)
                {
                    index = Mathf.Min(0, items.vehicles[workingSlot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count - 1);
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, items.vehicles[workingSlot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count - 1);
            }

            // Select the new module mount
            SelectModuleMount(index);
        }


        /// <summary>
        /// Remove modules from the currently selected module mount.
        /// </summary>
        public virtual void ClearSelectedModuleMount()
        {
            if (selectedModuleMountIndex != -1) loadoutData.SelectedSlot.selectedModules[selectedModuleMountIndex] = -1;

            OnLoadoutChanged();
        }



        public virtual ModuleMount GetSelectedModuleMount()
        {
            LoadoutVehicleItem vehicleItem = GetSelectedVehicleItem();
            if (vehicleItem == null) return null;

            if (selectedModuleMountIndex == -1) return null;

            return vehicleItem.vehiclePrefab.ModuleMounts[selectedModuleMountIndex];

        }

        // Update the list of modules that can be selected at the selected module mount
        protected virtual void UpdateSelectableModules()
        {
            selectableModuleIndexes.Clear();

            if (items == null) return;

            ModuleMount selectedModuleMount = GetSelectedModuleMount();
            if (selectedModuleMount == null) return;

            // Use predefined modules if any
            LoadoutVehicleItem vehicleItem = items.vehicles[workingSlot.selectedVehicleIndex];

            if (vehicleItem.predefinedModules != null && vehicleItem.predefinedModules.Count > 0)
            {
                foreach (var assignment in vehicleItem.predefinedModules)
                {
                    if (assignment.moduleMountIndex == selectedModuleMountIndex)
                    {
                        int index = items.modules.IndexOf(assignment.moduleItem);

                        if (index != -1)
                        {
                            selectableModuleIndexes.Add(index);
                        }
                    }
                }
            }
            else
            {
                // Fallback to normal logic if no predefined modules
                for (int i = 0; i < items.modules.Count; ++i)
                {
                    if (!selectedModuleMount.IsCompatible(items.modules[i].modulePrefab)) continue;

                    bool used = false;
                    if (exclusiveModules)
                    {
                        foreach (LoadoutSlot slot in loadoutData.Slots)
                        {
                            foreach (int usedModuleIndex in slot.selectedModules)
                            {
                                if (slot == loadoutData.SelectedSlot && slot.selectedModules.IndexOf(usedModuleIndex) == selectedModuleMountIndex) continue;
                                if (usedModuleIndex == i)
                                {
                                    used = true;
                                    break;
                                }
                            }
                            if (used) break;
                        }
                    }

                    if (!used) selectableModuleIndexes.Add(i);
                }
            }
        }



        // Module selection


        /// <summary>
        /// Select a module item.
        /// </summary>
        /// <param name="newModuleIndex">The index of the module to select (in the loadout items Modules list).</param>
        public virtual void SelectModule(int newModuleIndex)
        {
            workingSlot.selectedModules[selectedModuleMountIndex] = newModuleIndex;
            if (applyModuleSelection) SaveWorkingToActiveSlot();
            OnLoadoutChanged();
        }


        /// <summary>
        /// Cycle through the modules forward or back.
        /// </summary>
        /// <param name="forward">Whether to cycle forward (back if false)</param>
        /// <param name="wrap">Whether to wrap around to beginning when cycling past the end, or wrap to the end when cycling back past the beginning.</param>
        public virtual void CycleModule(bool forward, bool wrap = false)
        {

            if (selectableModuleIndexes.Count == 0) return;

            // Cycle forward or back
            int index = selectableModuleIndexes.IndexOf(workingSlot.selectedModules[selectedModuleMountIndex]);
            if (forward)
            {
                index++;
            }
            else
            {
                index--;
            }

            // Wrap
            if (wrap)
            {
                if (index < 0)
                {
                    index = selectableModuleIndexes.Count - 1;
                }
                else if (index >= selectableModuleIndexes.Count)
                {
                    index = Mathf.Min(0, selectableModuleIndexes.Count - 1);
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, selectableModuleIndexes.Count - 1);
            }

            // Select the new module
            SelectModule(selectableModuleIndexes[index]);
        }

        public void ApplyPredefinedModules()
        {
            LoadoutVehicleItem vehicleItem = GetSelectedVehicleItem();
            if (vehicleItem == null) return;

            for (int i = 0; i < vehicleItem.predefinedModules.Count; ++i)
            {
                var assignment = vehicleItem.predefinedModules[i];
                int moduleIndex = items.modules.IndexOf(assignment.moduleItem);
                if (moduleIndex != -1)
                {
                    workingSlot.selectedModules[assignment.moduleMountIndex] = moduleIndex;
                }
            }

            SaveWorkingToActiveSlot();
            OnLoadoutChanged();
        }


        // Data saving


        /// <summary>
        /// Save the working slot to the currently selected slot.
        /// </summary>
        public virtual void SaveWorkingToActiveSlot()
        {
            if (loadoutData.SelectedSlot == null) return;

            loadoutData.SelectedSlot.selectedVehicleIndex = workingSlot.selectedVehicleIndex;
            loadoutData.SelectedSlot.selectedModules = new List<int>(workingSlot.selectedModules);

            OnLoadoutChanged();
        }


        /// <summary>
        /// Revert the working slot to the currently selected slot.
        /// </summary>
        public virtual void RevertWorkingToActiveSlot()
        {
            if (loadoutData.SelectedSlot == null)
            {
                workingSlot.selectedVehicleIndex = -1;
                workingSlot.selectedModules = new List<int>();
                return;
            }

            workingSlot.selectedVehicleIndex = loadoutData.SelectedSlot.selectedVehicleIndex;
            workingSlot.selectedModules = new List<int>(loadoutData.SelectedSlot.selectedModules);

            OnLoadoutChanged();
        }


        /// <summary>
        /// Save the loadout data persistently.
        /// </summary>
        public virtual void SavePersistentData()
        {
            if (loadoutData != null)
            {
                loadoutDataManager.SaveData(loadoutData);
            }
            else
            {
                Debug.LogWarning("Failed to save data. Please set the Loadout Data component in the inspector.");
            }

            if (loadoutDataManager == null)
            {
                Debug.LogError("LoadoutDataManager is not assigned before attempting to save!");
                return;
            }

        }

        public void SelectFighterCategory(int categoryIndex)
        {
            if (items == null)
            {
                Debug.LogError("Loadout items not initialized yet!");
                return;
            }

            if (categoryIndex < 0 || categoryIndex >= fighterCategories.Count) return;

            selectedCategoryIndex = categoryIndex;
            selectedVariantLocalIndex = 0;

            selectableVariantIndexes.Clear();

            FighterCategory category = fighterCategories[categoryIndex];
            if (category == null) return;
            if (category.fighterVariants == null) return;

            for (int i = 0; i < category.fighterVariants.Count; i++)
            {
                int vehicleIndex = items.vehicles.IndexOf(category.fighterVariants[i]);

                if (vehicleIndex != -1)
                    selectableVariantIndexes.Add(vehicleIndex);
            }

            if (selectableVariantIndexes.Count > 0)
                SelectVehicle(selectableVariantIndexes[0]);

            OnLoadoutChanged();
        }

        public void CycleVariant(bool forward)
        {
            if (selectableVariantIndexes.Count == 0) return;

            if (forward)
                selectedVariantLocalIndex++;
            else
                selectedVariantLocalIndex--;

            if (selectedVariantLocalIndex < 0)
                selectedVariantLocalIndex = selectableVariantIndexes.Count - 1;

            if (selectedVariantLocalIndex >= selectableVariantIndexes.Count)
                selectedVariantLocalIndex = 0;

            SelectVehicle(selectableVariantIndexes[selectedVariantLocalIndex]);
        }

        public void CycleFighterCategory(bool forward)
        {
            if (fighterCategories.Count == 0) return;

            if (forward)
                selectedCategoryIndex++;
            else
                selectedCategoryIndex--;

            if (selectedCategoryIndex < 0)
                selectedCategoryIndex = fighterCategories.Count - 1;

            if (selectedCategoryIndex >= fighterCategories.Count)
                selectedCategoryIndex = 0;

            SelectFighterCategory(selectedCategoryIndex);
        }

        public void SelectVehicleVariant(int variantButtonIndex)
        {
            if (variantButtonIndex < 0 || variantButtonIndex >= selectableVariantIndexes.Count) return;

            selectedVariantLocalIndex = variantButtonIndex;

            int vehicleIndex = selectableVariantIndexes[variantButtonIndex];

            SelectVehicle(vehicleIndex);
        }



        public void SelectArcadeVisibleVehicle(int displayIndex)
        {
            if (arcadeVisibleVehicleIndexes.Count == 0) return;

            arcadeCurrentDisplayIndex = Mathf.Clamp(displayIndex, 0, arcadeVisibleVehicleIndexes.Count - 1);

            int realVehicleIndex = arcadeVisibleVehicleIndexes[arcadeCurrentDisplayIndex];

            // force bypass normal unlock index check
            workingSlot.selectedVehicleIndex = realVehicleIndex;

            List<int> defaultModules = GetDefaultModules(realVehicleIndex);
            workingSlot.selectedModules = new List<int>(defaultModules);

            SaveWorkingToActiveSlot();
            SelectModuleMount(0);
            OnLoadoutChanged();
        }

        public void CycleArcadeVisibleVehicle(bool forward, bool wrap = false)
        {
            if (arcadeVisibleVehicleIndexes.Count == 0) return;

            if (forward)
                arcadeCurrentDisplayIndex++;
            else
                arcadeCurrentDisplayIndex--;

            if (wrap)
            {
                if (arcadeCurrentDisplayIndex < 0)
                    arcadeCurrentDisplayIndex = arcadeVisibleVehicleIndexes.Count - 1;

                if (arcadeCurrentDisplayIndex >= arcadeVisibleVehicleIndexes.Count)
                    arcadeCurrentDisplayIndex = 0;
            }
            else
            {
                arcadeCurrentDisplayIndex = Mathf.Clamp(arcadeCurrentDisplayIndex, 0, arcadeVisibleVehicleIndexes.Count - 1);
            }

            SelectArcadeVisibleVehicle(arcadeCurrentDisplayIndex);
        }

        public int GetCurrentArcadeRealVehicleIndex()
        {
            if (arcadeVisibleVehicleIndexes.Count == 0) return -1;

            return arcadeVisibleVehicleIndexes[arcadeCurrentDisplayIndex];
        }

        public int GetCurrentArcadeDisplayIndex()
        {
            return arcadeCurrentDisplayIndex;
        }

        public int GetArcadeVisibleCount()
        {
            return arcadeVisibleVehicleIndexes.Count;
        }

        public int GetArcadeVehicleIndexByDisplay(int displayIndex)
        {
            if (displayIndex < 0 || displayIndex >= arcadeVisibleVehicleIndexes.Count) return -1;
            return arcadeVisibleVehicleIndexes[displayIndex];
        }

        public int GetArcadeTierFromVehicleIndex(int vehicleIndex)
        {
            return arcadeVisibleVehicleIndexes.IndexOf(vehicleIndex);
        }

        public int GetArcadeUnlockVehicleFromCompletedLevel(int completedLevel)
        {
            int unlockDisplayIndex = completedLevel + 1;

            if (unlockDisplayIndex < 0 || unlockDisplayIndex >= arcadeVisibleVehicleIndexes.Count)
                return -1;

            return arcadeVisibleVehicleIndexes[unlockDisplayIndex];
        }

        /// <summary>
        /// Load the saved loadout.
        /// </summary>
        public virtual void LoadPersistentData()
        {
            if (items == null) return;

            LoadoutData loadedData = loadoutDataManager.LoadData();

            if (loadedData != null)
            {
                if (loadedData.Slots.Count > 0)
                {
                    // Verify the vehicles

                    for (int i = 0; i < loadedData.Slots.Count; ++i)
                    {
                        if (loadedData.Slots[i].selectedVehicleIndex != -1)
                        {
                            // If the vehicle index is outside the range of the vehicles list, clear it
                            if (loadedData.Slots[i].selectedVehicleIndex >= items.vehicles.Count)
                            {
                                loadedData.Slots[i].selectedVehicleIndex = -1;
                            }
                            else
                            {
                               

                                // Check exclusivity
                                if (exclusiveVehicles)
                                {
                                    for (int j = 0; j < i; ++j)
                                    {
                                        if (loadedData.Slots[i].selectedVehicleIndex == loadedData.Slots[j].selectedVehicleIndex)
                                        {
                                            loadedData.Slots[i].selectedVehicleIndex = -1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Verify the number of module mounts

                    foreach (LoadoutSlot slot in loadedData.Slots)
                    {
                        if (slot.selectedVehicleIndex != -1)
                        {
                            int numModuleMountsOnVehicle = items.vehicles[slot.selectedVehicleIndex].vehiclePrefab.ModuleMounts.Count;
                            if (slot.selectedModules.Count > numModuleMountsOnVehicle)
                            {
                                slot.selectedModules.RemoveRange(numModuleMountsOnVehicle, slot.selectedModules.Count - numModuleMountsOnVehicle);
                            }
                            else if (slot.selectedModules.Count < numModuleMountsOnVehicle)
                            {
                                int numToAdd = numModuleMountsOnVehicle - slot.selectedModules.Count;
                                for (int i = 0; i < numToAdd; ++i)
                                {
                                    slot.selectedModules.Add(-1);
                                }
                            }
                        }
                        else
                        {
                            slot.selectedModules.Clear();
                        }
                    }

                    // Verify the modules

                    for (int i = 0; i < loadedData.Slots.Count; ++i)
                    {
                        if (loadedData.Slots[i].selectedVehicleIndex == -1) continue;

                        for (int j = 0; j < loadedData.Slots[i].selectedModules.Count; ++j)
                        {
                            if (loadedData.Slots[i].selectedModules[j] == -1) continue;

                            if (loadedData.Slots[i].selectedModules[j] >= items.modules.Count)
                            {
                                loadedData.Slots[i].selectedModules[j] = -1;
                                continue;
                            }

                            if (exclusiveModules)
                            {
                                for (int k = 0; k <= i; ++k)
                                {
                                    for (int l = 0; l < loadedData.Slots[k].selectedModules.Count; ++l)
                                    {
                                        if (k == i && l == j) continue;

                                        if (loadedData.Slots[k].selectedModules[l] == loadedData.Slots[i].selectedModules[j])
                                        {
                                            loadedData.Slots[i].selectedModules[j] = -1;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Verify the selected slot
                    if (loadedData.Slots.Count == 0)
                    {
                        loadedData.selectedSlotIndex = -1;
                    }
                    else
                    {
                        if (loadSelectedSlot)
                        {
                            if (loadedData.selectedSlotIndex < 0 || loadedData.selectedSlotIndex >= loadedData.Slots.Count)
                            {
                                loadedData.selectedSlotIndex = 0;
                            }
                        }
                        else
                        {
                            loadedData.selectedSlotIndex = 0;
                        }
                    }
                }
            }
            else
            {
                loadedData = new LoadoutData();
            }

            loadoutData = loadedData;

            OnDataLoad();

            SelectSlot(loadoutData.selectedSlotIndex);

            OnLoadoutChanged();
        }

        /// <summary>
        /// Delete the persistent data.
        /// </summary>
        /// <param name="loadDefaultData">Whether to load default loadout data after deleting saved loadout data.</param>
        public virtual void DeletePersistentData(bool loadDefaultData = true)
        {
            loadoutDataManager.DeleteSaveData();
            if (loadDefaultData)
            {
                LoadDefaultData();
            }
        }
    }
}