using UnityEngine;
using VSX.UniversalVehicleCombat;

public class SceneMissionController : MonoBehaviour
{
    public SpaceshipWarpSpawn enemySpawnPoint; // Assign in Inspector
    public GameObject gunEnemyPrefab;
    public GameObject jianEnemyPrefab;
    //public GameObject qiangEnemyPrefab;
    //public GameObject guandaoEnemyPrefab;

    void Start()
    {
        string enemyType = MissionParameters.Instance.enemyType;

        GameObject selectedPrefab = null;
        switch (enemyType)
        {
            case "Gun": selectedPrefab = gunEnemyPrefab; break;
            case "Jian": selectedPrefab = jianEnemyPrefab; break;
            //case "Qiang": selectedPrefab = qiangEnemyPrefab; break;
            //case "Guandao": selectedPrefab = guandaoEnemyPrefab; break;
        }

        if (selectedPrefab != null)
        {
            enemySpawnPoint.spawnableVehiclePrefab = selectedPrefab.GetComponent<Vehicle>();
            enemySpawnPoint.Spawn();
        }

        Debug.Log("Mission Time: " + MissionParameters.Instance.missionTime);
        Debug.Log("Ammo Type: " + MissionParameters.Instance.ammo);
    }
}
