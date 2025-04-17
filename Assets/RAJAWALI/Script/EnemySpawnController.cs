using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

public class EnemySpawnController : MonoBehaviour
{
    public WaveController waveController;

    [Header("Enemy Spawner Prefabs")]
    public PilotedVehicleSpawn gunSpawnerPrefab;
    public PilotedVehicleSpawn jianSpawnerPrefab;
    //public PilotedVehicleSpawn qiangSpawnerPrefab;
    //public PilotedVehicleSpawn guandaoSpawnerPrefab;

    public Transform spawnerParent;

    private void Start()
    {
        string enemyType = MissionManager.Instance.currentMission.enemyType;

        List<PilotedVehicleSpawn> selectedSpawners = new List<PilotedVehicleSpawn>();

        PilotedVehicleSpawn prefabToSpawn = null;

        switch (enemyType)
        {
            case "Gun":
                prefabToSpawn = gunSpawnerPrefab;
                break;
            case "Jian":
                prefabToSpawn = jianSpawnerPrefab;
                break;
            /*case "Qiang":
                prefabToSpawn = qiangSpawnerPrefab;
                break;
            case "Guandao":
                prefabToSpawn = guandaoSpawnerPrefab;
                break; */
        }

        // Optional: spawn 3 enemies of selected type
        for (int i = 0; i < 3; i++)
        {
            PilotedVehicleSpawn spawner = Instantiate(prefabToSpawn, spawnerParent);
            selectedSpawners.Add(spawner);
        }

        waveController.Spawners.Clear(); // Clear any inspector-assigned spawners
        waveController.Spawners.AddRange(selectedSpawners);
        waveController.Spawn();
    }
}