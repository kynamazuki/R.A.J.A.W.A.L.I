using UnityEngine;
using UnityEngine.UI;
using VSX.UniversalVehicleCombat.Loadout;
using VSX.Utilities.UI;

public class FighterStatUI : MonoBehaviour
{
    [Header("Screen UI Sets")]
    [SerializeField] private FighterStatUIScreenSet normalUI;
    [SerializeField] private FighterStatUIScreenSet tripleUI;

    private Slider armorSlider;
    private Slider shieldSlider;
    private Slider speedSlider;
    private Slider agilitySlider;

    private UVCText nameText;
    private UVCText descriptionText;

    private void Awake()
    {
        ApplyUIScreenSet();
    }

    void ApplyUIScreenSet()
    {
        float ratio = (float)Screen.width / Screen.height;

        FighterStatUIScreenSet selectedSet = ratio > 4.0f ? tripleUI : normalUI;

        if (selectedSet == null)
        {
            Debug.LogError("FighterStatUIScreenSet not assigned!");
            return;
        }

        armorSlider = selectedSet.armorSlider;
        shieldSlider = selectedSet.shieldSlider;
        speedSlider = selectedSet.speedSlider;
        agilitySlider = selectedSet.agilitySlider;

        nameText = selectedSet.nameText;
        descriptionText = selectedSet.descriptionText;
    }

    public void DisplayStats(LoadoutVehicleItem vehicleItem)
    {
        if (vehicleItem == null) return;

        if (nameText != null)
            nameText.text = vehicleItem.Label;

        if (descriptionText != null)
            descriptionText.text = "Combat Ready";

        if (armorSlider != null)
            armorSlider.value = vehicleItem.armor;

        if (shieldSlider != null)
            shieldSlider.value = vehicleItem.shield;

        if (speedSlider != null)
            speedSlider.value = vehicleItem.speed;

        if (agilitySlider != null)
            agilitySlider.value = vehicleItem.agility;
    }

    public void DisplayLocked(int requiredLevel)
    {
        if (nameText != null)
            nameText.text = "LOCKED";

        if (descriptionText != null)
            descriptionText.text = "Unlock at Level " + requiredLevel;

        if (armorSlider != null)
            armorSlider.value = 0;

        if (shieldSlider != null)
            shieldSlider.value = 0;

        if (speedSlider != null)
            speedSlider.value = 0;

        if (agilitySlider != null)
            agilitySlider.value = 0;
    }
}