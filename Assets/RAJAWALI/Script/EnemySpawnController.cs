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

    private List<PilotedVehicleSpawn> selectedSpawners;

    private void Start()
    {
        selectedSpawners = new List<PilotedVehicleSpawn>();
        // Select the spawner prefab based on enemy type
        string enemyType = MissionManager.Instance.currentMission.enemyType;
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

        SpawnWaveEnemies(prefabToSpawn);
    }

    public void SpawnWaveEnemies(PilotedVehicleSpawn prefabToSpawn)
    {
        // Clear previous spawners if necessary
        foreach (var spawner in selectedSpawners)
        {
            Destroy(spawner.gameObject);
        }
        selectedSpawners.Clear();

        // Spawn 3 enemies for the wave
        for (int i = 0; i < 3; i++)
        {
            PilotedVehicleSpawn spawner = Instantiate(prefabToSpawn, spawnerParent);
            selectedSpawners.Add(spawner);
        }

        // Add spawners to the wave controller
        waveController.Spawners.Clear();
        waveController.Spawners.AddRange(selectedSpawners);
        waveController.Spawn();
    }

}