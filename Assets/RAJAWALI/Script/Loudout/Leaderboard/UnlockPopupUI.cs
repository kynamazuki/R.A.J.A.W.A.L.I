using UnityEngine;
using TMPro;
using VSX.UniversalVehicleCombat.Loadout;
using System.Collections;
using System.Collections.Generic;

public class UnlockPopupUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text unlockText;
    public TMP_Text scoreText;

    private bool isFinalCampaignPopup = false;

    IEnumerator Start()
    {
        while (LoadoutManager.Instance == null)
            yield return null;

        while (LoadoutManager.Instance.Items == null)
            yield return null;

        while (LoadoutManager.Instance.LoadoutData == null || LoadoutManager.Instance.LoadoutData.Slots == null)
            yield return null;

        yield return null;

        CheckAndShow();
    }

    void CheckAndShow()
    {
        var loadout = LoadoutManager.Instance.LoadoutData;

        if (loadout.showFinalLeaderboard)
        {
            isFinalCampaignPopup = true;

            unlockText.text = "ALL LEVELS COMPLETE!";
            scoreText.text = "Final Score: " + LeaderboardManager.Instance.currentScore;

            panel.SetActive(true);

            return;
        }

        if (!loadout.showUnlockPopup) return;

        int unlockedIndex = LoadoutManager.Instance.GetArcadeUnlockVehicleFromCompletedLevel(loadout.lastCompletedMissionIndex);

        if (unlockedIndex != -1 &&
            LoadoutManager.Instance.Items != null &&
            unlockedIndex < LoadoutManager.Instance.Items.vehicles.Count)
        {
            string fighterName = LoadoutManager.Instance.Items.vehicles[unlockedIndex].Label;
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

        var loadout = LoadoutManager.Instance.LoadoutData;

        // If this was final campaign popup
        if (isFinalCampaignPopup)
        {
            isFinalCampaignPopup = false;

            loadout.showFinalLeaderboard = false;
            loadout.showUnlockPopup = false;
            LoadoutManager.Instance.SavePersistentData();

            if (PlayerProfileUI.Instance != null)
            {
                PlayerProfileUI.Instance.ShowAfterDeath();
            }
        }
    }
}