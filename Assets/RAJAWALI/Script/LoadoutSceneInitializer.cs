using UnityEngine;
using VSX.UniversalVehicleCombat.Loadout;

public class LoadoutSceneInitializer : MonoBehaviour
{
    public LoadoutUIController loadoutUIController;

    private void Start()
    {
        if (loadoutUIController != null)
        {
            loadoutUIController.EnterVehicleSelection();
            loadoutUIController.OnLoadoutChanged(); 
        }

        // Optionally clear working loadout to force fresh selection
        LoadoutManager loadoutManager = FindObjectOfType<LoadoutManager>();
        if (loadoutManager != null)
        {
            loadoutManager.SaveWorkingToActiveSlot();
        }
    }
}
