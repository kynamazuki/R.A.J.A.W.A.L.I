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
        if (currentMission == null)
        {
            Debug.LogError("Mission not set! Cannot start mission.");
            return;
        }

        PlayerPrefs.SetFloat("MissionTime", currentMission.missionTime);

        // Format: Location_MissionType
        string sceneName = "";

        switch (currentMission.location)
        {
            case "Deep Space":
                sceneName += "DeepSpace_";
                break;
            case "Asteroid Field":
                sceneName += "AsteroidField_";
                break;
            case "Capital Ship Battle":
                sceneName += "CapitalShip_";
                break;
            default:
                Debug.LogError("Unknown location: " + currentMission.location);
                return;
        }

        switch (currentMission.missionType)
        {
            case "Supremacy":
                sceneName += "Supreme";
                break;
            case "Strike":
                sceneName += "Strike";
                break;
            case "Defend":
                sceneName += "Defend";
                break;
            default:
                Debug.LogError("Unknown mission type: " + currentMission.missionType);
                return;
        }

        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
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
