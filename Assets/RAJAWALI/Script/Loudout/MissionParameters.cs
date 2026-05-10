using UnityEngine;

public class MissionParameters : MonoBehaviour
{
    public static MissionParameters Instance { get; private set; }

    public string missionType;
    public float missionTime = 300f; // Default 5 minutes
    public bool timerEnabled = true;
    public string enemyType;
    public string location;

    public string selectedFighter;
    public string selectedWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);  
            return;
        }


        Instance = this;
        DontDestroyOnLoad(this.gameObject);  
    }


}
