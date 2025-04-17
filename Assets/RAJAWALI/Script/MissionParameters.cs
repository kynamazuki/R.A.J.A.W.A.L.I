using UnityEngine;

public class MissionParameters : MonoBehaviour
{
    public static MissionParameters Instance { get; private set; }

    public string missionType;
    public float missionTime;
    public string enemyType;
    public string ammo;
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
