using UnityEngine;
using UnityEngine.SceneManagement;
using VSX.UniversalVehicleCombat.Loadout;
using VSX.UniversalVehicleCombat;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public LoadoutManager loadoutManager;

    [Header("Reference to the current mission parameters")]
    public MissionParameters currentMission;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optional, if you want it to persist
        }
        else
        {
            Destroy(gameObject); // to prevent duplicates
        }
    }

    public void StartMission()
    {
        loadoutManager.SavePersistentData();
        switch (currentMission.location)
        {
            case "Deep Space":
                SceneManager.LoadScene("DeepSpace");
                break;
            case "Asteroid Field":
                SceneManager.LoadScene("AsteroidField");
                break;
            case "Capital Ship Battle":
                SceneManager.LoadScene("CapitalShip");
                break;
            default:
                Debug.LogWarning("Unknown location. Loading default scene.");
                SceneManager.LoadScene("DefaultScene");
                break;
        }


    }



}

