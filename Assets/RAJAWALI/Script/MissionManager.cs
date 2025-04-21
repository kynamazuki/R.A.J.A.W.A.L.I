using UnityEngine.SceneManagement;
using UnityEngine;
using VSX.UniversalVehicleCombat.Loadout;
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
