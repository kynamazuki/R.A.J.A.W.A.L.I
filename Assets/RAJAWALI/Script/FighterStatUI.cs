using UnityEngine;
using UnityEngine.UI;
using VSX.UniversalVehicleCombat.Loadout;
using TMPro;

public class FighterStatUI : MonoBehaviour
{
    public Slider armorSlider;
    public Slider shieldSlider;
    public Slider speedSlider;
    public Slider agilitySlider;

    [Header("Text UI")]
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public void DisplayStats(LoadoutVehicleItem vehicleItem)
    {
        nameText.text = vehicleItem.Label;
        descriptionText.text = "Combat Ready";

        armorSlider.value = vehicleItem.armor;
        shieldSlider.value = vehicleItem.shield;
        speedSlider.value = vehicleItem.speed;
        agilitySlider.value = vehicleItem.agility;
    }

    public void DisplayLocked(int requiredLevel)
    {
        // Show LOCK state
        nameText.text = "LOCKED";
        descriptionText.text = "Unlock at Level " + requiredLevel;

        // Reset sliders (visual feedback)
        armorSlider.value = 0;
        shieldSlider.value = 0;
        speedSlider.value = 0;
        agilitySlider.value = 0;
    }
}
