using UnityEngine;
using TMPro;
using VSX.UniversalVehicleCombat.Loadout;

public class UnlockPopupUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text unlockText;
    public TMP_Text scoreText;

    void Start()
    {
        CheckAndShow();
    }

    void CheckAndShow()
    {
        var loadout = LoadoutManager.Instance.LoadoutData;

        if (!loadout.showUnlockPopup) return;

        int unlockedIndex = loadout.lastCompletedMissionIndex + 1;

        string fighterName = "Unknown";

        if (LoadoutManager.Instance.Items != null &&
            unlockedIndex < LoadoutManager.Instance.Items.vehicles.Count)
        {
            fighterName = LoadoutManager.Instance.Items.vehicles[unlockedIndex].Label;
        }

        unlockText.text = fighterName + " Fighter Unlocked!";
        scoreText.text = "Score: " + LeaderboardManager.Instance.currentScore;

        panel.SetActive(true);

        // ❗ IMPORTANT: reset so it doesn't show again
        loadout.showUnlockPopup = false;
        LoadoutManager.Instance.SavePersistentData();
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}