using UnityEngine;
using VSX.UniversalVehicleCombat;

public class PlayerDeathHandler : MonoBehaviour
{
    private Damageable damageable;

    private void Awake()
    {
        // FIX: search in children
        damageable = GetComponentInChildren<Damageable>();

        if (damageable == null)
        {
            Debug.LogError(" No Damageable found on Player!");
        }
        else
        {
            Debug.Log(" Damageable found: " + damageable.name);
        }
    }

    private void Start()
    {
        if (damageable != null)
        {
            damageable.onDestroyed.AddListener(OnPlayerDestroyed);
        }
    }

    void OnPlayerDestroyed()
    {
        Debug.Log("PLAYER DESTROYED (Damageable)");

        if (PlayerProfileUI.Instance != null)
        {
            PlayerProfileUI.Instance.ShowAfterDeath();
        }
    }

    System.Collections.IEnumerator ShowUI()
    {
        yield return null;

        if (PlayerProfileUI.Instance != null)
            PlayerProfileUI.Instance.ShowNamePanel();
        else
            Debug.LogError("PlayerProfileUI.Instance NULL");

        if (LeaderboardUI.Instance != null)
            LeaderboardUI.Instance.ShowLeaderboard();
        else
            Debug.LogError("LeaderboardUI.Instance NULL");
    }
}