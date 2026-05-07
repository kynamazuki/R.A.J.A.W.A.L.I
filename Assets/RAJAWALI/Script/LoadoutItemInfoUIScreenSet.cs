using UnityEngine;
using UnityEngine.UI;
using VSX.Utilities.UI;

namespace VSX.UniversalVehicleCombat.Loadout
{
    [System.Serializable]
    public class LoadoutItemInfoUIScreenSet
    {
        public GameObject UIHandle;
        public UVCText labelText;
        public UVCText descriptionText;
        public Image iconImage;
        public Transform statsInstanceParent;
    }
}