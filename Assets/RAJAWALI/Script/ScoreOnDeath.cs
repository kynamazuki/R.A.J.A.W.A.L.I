using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Loadout;

public class ScoreOnDeath : MonoBehaviour
{
    private GameAgent agent;
    private Damageable damageable;

    private void Awake()
    {
        damageable = GetComponentInChildren<Damageable>();

        if (damageable == null)
        {
            Debug.LogError("No Damageable found: " + gameObject.name);
        }
    }

    private void Start()
    {
        if (damageable != null)
        {
            damageable.onDestroyed.AddListener(OnDestroyed);
        }
    }

    public void OnDestroyed()
    {
        Vehicle vehicle = GetComponent<Vehicle>();

        if (vehicle == null) return;
        if (vehicle.Occupants == null || vehicle.Occupants.Count == 0) return;

        GameAgent agent = vehicle.Occupants[0];

        if (agent == null || agent.IsPlayer) return;

        // ADD SCORE HERE
        LeaderboardManager.Instance.currentScore += 1;

        // Update HUD
        if (ScoreHUD.Instance != null)
        {
            ScoreHUD.Instance.UpdateScoreDisplay(
                LeaderboardManager.Instance.currentScore
            );
        }

        Debug.Log(" SCORE ADDED: " + LeaderboardManager.Instance.currentScore);
    }
}