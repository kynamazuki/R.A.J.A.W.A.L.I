using UnityEngine;
using VSX.UniversalVehicleCombat.Loadout;

public class SessionExitHandler : MonoBehaviour
{
    public LoadoutManager loadoutManager;

    private void OnApplicationQuit()
    {
        LeaderboardManager.Instance.SaveCurrentSession();
    }
}