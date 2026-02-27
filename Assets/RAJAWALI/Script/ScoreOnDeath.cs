using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Loadout;

public class ScoreOnDeath : MonoBehaviour
{
    [SerializeField] private Team playerTeam;

    private GameAgent agent;
    private Vehicle vehicle;

    private void Awake()
    {
        agent = GetComponent<GameAgent>();
    }

    private void Start()
    {
        if (agent != null && agent.Vehicle != null)
        {
            vehicle = agent.Vehicle;
            vehicle.onDestroyed.AddListener(OnDestroyed);
        }
    }

    private void OnDestroyed()
    {
        if (agent.Team != null && agent.Team != playerTeam)
        {
            LoadoutManager lm = FindObjectOfType<LoadoutManager>();
            if (lm != null)
            {
                lm.LoadoutData.totalScore += 1;
                lm.SavePersistentData(); // Save current score to JSON

                // Update live leaderboard
                LeaderboardManager.Instance.UpdateCurrentScore(lm.LoadoutData.totalScore);

                LeaderboardUI.Instance.Refresh();

                // Update HUD if you have a reference to it
                if (ScoreHUD.Instance != null)
                {
                    ScoreHUD.Instance.UpdateScoreDisplay(lm.LoadoutData.totalScore);
                }
            }
        }
    }
}