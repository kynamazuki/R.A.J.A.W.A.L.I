using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

public class EnemySpawnController : MonoBehaviour
{
    public WavesController wavesController;

    [Header("Enemy Spawner Prefabs")]
    public PilotedVehicleSpawn gunSpawnerPrefab;
    public PilotedVehicleSpawn jianSpawnerPrefab;
    // public PilotedVehicleSpawn qiangSpawnerPrefab;
    // public PilotedVehicleSpawn guandaoSpawnerPrefab;

    public Transform spawnerParent;

    private List<PilotedVehicleSpawn> selectedSpawners = new List<PilotedVehicleSpawn>();

    private void Start()
    {
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
                // case "Qiang":
                //     prefabToSpawn = qiangSpawnerPrefab;
                //     break;
                // case "Guandao":
                //     prefabToSpawn = guandaoSpawnerPrefab;
                //     break;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("Enemy prefab not assigned for selected type.");
            return;
        }

        // Spawn for all waves
        foreach (WaveController wave in wavesController.WaveControllers)
        {
            SpawnWaveEnemies(prefabToSpawn, wave);
        }
    }

    public void SpawnWaveEnemies(PilotedVehicleSpawn prefabToSpawn, WaveController wave)
    {
        List<PilotedVehicleSpawn> spawnersForThisWave = new List<PilotedVehicleSpawn>();

        for (int i = 0; i < 3; i++)
        {
            PilotedVehicleSpawn spawner = Instantiate(prefabToSpawn, spawnerParent);
            spawnersForThisWave.Add(spawner);
        }

        wave.Spawners.Clear();
        wave.Spawners.AddRange(spawnersForThisWave);

        foreach (PilotedVehicleSpawn s in spawnersForThisWave)
        {
            s.onDestroyed.AddListener(wave.OnWaveMemberDestroyed);
        }
    }
}
