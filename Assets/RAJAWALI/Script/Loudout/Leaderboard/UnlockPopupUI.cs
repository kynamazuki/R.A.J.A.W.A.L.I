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

        int completedLevel = loadout.lastCompletedMissionIndex;



        // Arcade visible fighter unlock mapping
        int unlockedIndex = LoadoutManager.Instance.GetArcadeUnlockVehicleFromCompletedLevel(loadout.lastCompletedMissionIndex);

        string fighterName = "";

        if (unlockedIndex != -1 &&
            LoadoutManager.Instance.Items != null &&
            unlockedIndex < LoadoutManager.Instance.Items.vehicles.Count)
        {
            fighterName = LoadoutManager.Instance.Items.vehicles[unlockedIndex].Label;
            unlockText.text = fighterName + " Fighter Unlocked!";
        }
        else
        {
            unlockText.text = "Mission Complete!";
        }

        scoreText.text = "Score: " + LeaderboardManager.Instance.currentScore;

        panel.SetActive(true);

        loadout.showUnlockPopup = false;
        LoadoutManager.Instance.SavePersistentData();
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}