using UnityEngine;
using VSX.Utilities.UI;

[System.Serializable]
public class LoadoutUIScreenSet
{
    public GameObject rootCanvas;

    public GameObject vehicleSelectionModeUIHandle;
    public GameObject vehicleInfoUIHandle;
    public GameObject selectPreviousVehicleButton;
    public GameObject selectNextVehicleButton;
    public GameObject selectPreviousVariantButton;
    public GameObject selectNextVariantButton;
    public GameObject equipVehicleButton;
    public GameObject goToLoadoutButton;

    public GameObject moduleSelectionModeUIHandle;
    public GameObject moduleOptionsUIHandle;
    public ButtonsListController moduleButtonsListController;

    public ButtonsListController moduleMountButtonsListController;
    public GameObject moduleMountOptionsUIHandle;

    public ButtonsListController slotButtonsListController;
    public Transform slotSelectionUIHandle;

    public ButtonsListController fighterCategoryButtonsListController;
    public ButtonsListController fighterVariantButtonsListController;
    public GameObject fighterVariantPanel;

    public GameObject MissionParameterUIHandle;

    [SerializeField] public ButtonController launchButton;
}