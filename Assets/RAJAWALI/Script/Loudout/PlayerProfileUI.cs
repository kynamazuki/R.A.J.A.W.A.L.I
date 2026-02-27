using UnityEngine;
using TMPro;
using VSX.UniversalVehicleCombat.Loadout;

public class PlayerProfileUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject namePanel;
    public LoadoutManager loadoutManager;

    public void OnEnterPressed()
    {
        if (string.IsNullOrEmpty(nameInput.text)) return;

        // Update LoadoutData fields without replacing reference
        loadoutManager.LoadoutData.playerName = nameInput.text;
        loadoutManager.LoadoutData.totalScore = 0; // NEW SESSION START
        loadoutManager.LoadoutData.currentMissionIndex = 0;
        loadoutManager.SavePersistentData();

        // Start live leaderboard session
        LeaderboardManager.Instance.StartNewSession(loadoutManager.LoadoutData.playerName);

        namePanel.SetActive(false); // hide name panel
    }
}