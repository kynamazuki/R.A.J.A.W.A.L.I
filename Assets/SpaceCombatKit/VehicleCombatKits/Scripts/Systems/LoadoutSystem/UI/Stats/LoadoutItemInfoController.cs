using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VSX.Utilities.UI;

namespace VSX.UniversalVehicleCombat.Loadout
{
    /// <summary>
    /// Base class to manage the display of info for a loadout item.
    /// </summary>
    public class LoadoutItemInfoController : MonoBehaviour
    {
        [Header("UI Screen Sets")]
        [SerializeField] protected LoadoutItemInfoUIScreenSet normalUI;
        [SerializeField] protected LoadoutItemInfoUIScreenSet tripleUI;

        protected GameObject UIHandle;
        protected UVCText labelText;
        protected UVCText descriptionText;
        protected Image iconImage;
        protected Transform statsInstanceParent;

        [SerializeField]
        protected LoadoutManager loadoutManager;

        protected List<LoadoutItemInfoOverrideController> overrideControllers = new List<LoadoutItemInfoOverrideController>();

        [Header("Stats Controller")]
        [SerializeField]
        protected StatsInstance statsInstancePrefab;

        protected List<StatsInstance> statsInstances = new List<StatsInstance>();

        protected virtual void Reset()
        {
            loadoutManager = FindObjectOfType<LoadoutManager>();
        }

        protected virtual void Awake()
        {
            ApplyUIScreenSet();

            if (LoadoutManager.Instance != null)
                loadoutManager = LoadoutManager.Instance;
            else
                loadoutManager = FindObjectOfType<LoadoutManager>();

            if (loadoutManager == null)
            {
                Debug.LogError("LoadoutManager not found in LoadoutItemInfoController!");
                return;
            }

            overrideControllers = new List<LoadoutItemInfoOverrideController>(
                transform.GetComponentsInChildren<LoadoutItemInfoOverrideController>());

            foreach (LoadoutItemInfoOverrideController overrideController in overrideControllers)
            {
                overrideController.ItemInfoController = this;
            }

            loadoutManager.onLoadoutChanged.AddListener(UpdateInfo);
        }


        protected virtual void Show()
        {
            if (UIHandle != null)
                UIHandle.SetActive(true);
        }

        protected virtual void Hide()
        {
            if (UIHandle != null)
                UIHandle.SetActive(false);
        }


        /// <summary>
        /// Update the loadout item info.
        /// </summary>
        public virtual void UpdateInfo()
        {
            ClearStatsInstances();
            Show();
        }


        /// <summary>
        /// Get a new stats instance to display something about the loadout item.
        /// </summary>
        /// <returns></returns>
        public virtual StatsInstance GetStatsInstance()
        {
            foreach(StatsInstance statsInstance in statsInstances)
            {
                if (!statsInstance.gameObject.activeSelf)
                {
                    statsInstance.gameObject.SetActive(true);
                    return statsInstance;
                }
            }

            StatsInstance newStatsInstance = Instantiate(statsInstancePrefab, statsInstanceParent);
            statsInstances.Add(newStatsInstance);

            return newStatsInstance;
        }


        /// <summary>
        /// Clear all the stats items.
        /// </summary>
        public virtual void ClearStatsInstances()
        {
            foreach (StatsInstance statsInstance in statsInstances)
            {
                statsInstance.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Set the label for the loadout item.
        /// </summary>
        /// <param name="text">The label content.</param>
        public virtual void SetLabel(string text)
        {
            if (labelText != null) labelText.text = text;
        }


        /// <summary>
        /// Set the description for the loadout item.
        /// </summary>
        /// <param name="text">The description content.</param>
        public virtual void SetDescription(string text)
        {
            if (descriptionText != null) descriptionText.text = text;
        }


        /// <summary>
        /// Set the icon sprite for the loadout item.
        /// </summary>
        /// <param name="icon">The icon sprite.</param>
        public virtual void SetIcon(Sprite icon)
        {
            if (iconImage != null) iconImage.sprite = icon;
        }

        void ApplyUIScreenSet()
        {
            float ratio = (float)Screen.width / Screen.height;

            LoadoutItemInfoUIScreenSet selectedSet = ratio > 4.0f ? tripleUI : normalUI;

            if (selectedSet == null)
            {
                Debug.LogError("LoadoutItemInfoUIScreenSet not assigned!");
                return;
            }

            UIHandle = selectedSet.UIHandle;
            labelText = selectedSet.labelText;
            descriptionText = selectedSet.descriptionText;
            iconImage = selectedSet.iconImage;
            statsInstanceParent = selectedSet.statsInstanceParent;
        }
    }
}

