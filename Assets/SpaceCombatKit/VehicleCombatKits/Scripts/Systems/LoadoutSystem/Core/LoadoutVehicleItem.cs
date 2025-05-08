using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.ResourceSystem;

namespace VSX.UniversalVehicleCombat.Loadout
{
    public class LoadoutVehicleItem : MonoBehaviour
    {
        [Tooltip("The vehicle prefab associated with this loadout item.")]
        public Vehicle vehiclePrefab;

        [Tooltip("Whether to override the label of the Vehicle component when displaying this vehicle in the loadout menu.")]
        public bool overrideVehicleLabel = false;

        [Tooltip("The label value to display in the loadout menu when overriding the label on the Vehicle component.")]
        public string overrideLabel = "Label Override";

        public virtual string Label
        {
            get { return overrideVehicleLabel ? overrideLabel : vehiclePrefab.Label; }
        }

        [Tooltip("The description to display in the loadout menu for this vehicle.")]
        [TextArea]
        public string description;

        [Range(0, 10000)]
        public float armor = 50;

        [Range(0, 10000)]
        public float shield = 60;

        [Range(0, 1000)]
        public float speed = 70;

        [Range(0, 10)]
        public float agility = 5;

        [Tooltip("All the sprites associated with this vehicle (can be looked up by index).")]
        public List<Sprite> sprites = new List<Sprite>();

        [Tooltip("The default module loadout for the vehicle (displayed when no saved data is found).")]
        public List<Module> defaultLoadout = new List<Module>();

       /* [Header("Resource Container")]
        [Tooltip("The prefab for this resource container (e.g. ammo, fuel).")]
        public ResourceContainer weaponPrefab;

        [Tooltip("Whether to override the capacity and start amount of the resource container.")]
        public bool overrideResourceAmmo = false;

        [Tooltip("The overridden capacity value.")]
        public float overrideCapacityFloat = 15f;

        [Tooltip("The overridden current amount value.")]
        public float overrideCurrentAmountFloat = 15f;

        public virtual float Capacity
        {
            get
            {
                if (overrideResourceAmmo)
                    return overrideCapacityFloat;
                else
                    return weaponPrefab != null ? weaponPrefab.CapacityFloat : 0f;
            }
        }

        public virtual float StartAmount
        {
            get
            {
                if (overrideResourceAmmo)
                    return overrideCurrentAmountFloat;
                else
                    return weaponPrefab != null ? weaponPrefab.CurrentAmountFloat : 0f;
            }
        } */

        [Tooltip("Whether this vehicle is currently locked and unavailable, or is unlocked and available in the loadout.")]
        public bool locked = false;

        [Tooltip("The offset for the camera from the module mount position when focusing on a module on this vehicle.")]
        public Vector3 moduleMountViewAlignmentOffset = Vector3.zero;




    }

}
