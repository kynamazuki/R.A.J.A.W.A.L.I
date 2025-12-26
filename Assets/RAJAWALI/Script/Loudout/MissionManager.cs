using UnityEngine.SceneManagement;
using UnityEngine;
using VSX.UniversalVehicleCombat.Loadout;
using TMPro;
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public LoadoutManager loadoutManager;
    public LoadoutUIController loadoutUIController;

    public MissionParameters currentMission;


    protected LoadoutData loadoutData;
    public LoadoutData LoadoutData { get { return loadoutData; } }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // Find and assign the LoadoutDataManager to LoadoutManager
        LoadoutDataManager foundDataManager = FindObjectOfType<LoadoutDataManager>();
        
        loadoutUIController.EnterVehicleSelection();
        loadoutUIController.OnLoadoutChanged();

        
    }


    public void StartMission()
    {
        Debug.Log("Launch Button Clicked. Starting mission...");

        currentMission = MissionParameters.Instance;

        if (currentMission == null)
        {
            Debug.LogError("Mission not set! Cannot start mission.");
            return;
        }


        PlayerPrefs.SetFloat("MissionTime", currentMission.missionTime);

        string sceneName = GetSceneNameByLocation(currentMission.location);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Invalid mission location!");
            return;
        }

        Debug.Log($"Loading Scene: {sceneName} | Mission Type: {currentMission.missionType}");
        SceneManager.LoadScene(sceneName);
    }

    private string GetSceneNameByLocation(string location)
    {
        switch (location)
        {
            case "Deep Space":
                return "DeepSpace";

            case "Asteroid Field":
                return "AsteroidField";

            case "Capital Ship Battle":
                return "CapitalShipBattle";

            default:
                Debug.LogError($"Unknown location: {location}");
                return null;
        }
    }


    public void ResetMission()
    {
        currentMission = MissionParameters.Instance;

        if (currentMission != null)
        {
            currentMission.enemyType = null;
            currentMission.ammo = null;
            currentMission.location = null;
            currentMission.missionType = null;
            currentMission.missionTime = 0;
        }
    }
}
