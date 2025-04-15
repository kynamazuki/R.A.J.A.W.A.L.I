using UnityEngine;

[CreateAssetMenu(menuName = "Mission/MissionParameters")]
public class MissionParameters : ScriptableObject
{
    public string missionType;
    public float missionTime;
    public string enemyType;
    public string ammoType;
    public string location;
}
