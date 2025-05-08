using UnityEngine;
using UnityEngine.UI;
using VSX.UniversalVehicleCombat.Loadout;

public class FighterStatUI : MonoBehaviour
{
    public Slider armorSlider;
    public Slider shieldSlider;
    public Slider speedSlider;
    public Slider agilitySlider;

    public void DisplayStats(LoadoutVehicleItem vehicleItem)
    {
        armorSlider.value = vehicleItem.armor;
        shieldSlider.value = vehicleItem.shield;
        speedSlider.value = vehicleItem.speed;
        agilitySlider.value = vehicleItem.agility;
    }
}
