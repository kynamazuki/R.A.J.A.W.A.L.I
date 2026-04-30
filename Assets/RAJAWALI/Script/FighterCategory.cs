using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Loadout
{
    [CreateAssetMenu(menuName = "RAJAWALI/Fighter Category")]
    public class FighterCategory : ScriptableObject
    {
        public string categoryName;
        public Sprite categoryIcon;

        public List<LoadoutVehicleItem> fighterVariants = new List<LoadoutVehicleItem>();
    }
}