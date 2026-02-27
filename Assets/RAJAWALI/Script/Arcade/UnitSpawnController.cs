/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;

public class UnitSpawnController : MonoBehaviour
{
    [Header("Teams")]
    public Team enemyTeam;
    public Team playerTeam;

    [Header("Wave Parents")]
    public Transform enemyParent;
    public Transform allyParent;

    [Header("Waves")]
    public List<UnitWaveDefinition> waves;

    private int aliveUnits = 0;

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        foreach (var wave in waves)
        {
            SpawnWave(wave);
            yield return new WaitUntil(() => aliveUnits <= 0);
            yield return new WaitForSeconds(wave.waveInterval);
        }

        Debug.Log("All waves complete!");
    }

    void SpawnWave(UnitWaveDefinition waveDef)
    {
        if (waveDef == null || waveDef.spawnerPrefabs == null || waveDef.spawnerPrefabs.Count == 0)
        {
            Debug.LogError("Wave definition or spawnerPrefabs is missing!");
            return;
        }

        Transform parent = waveDef.isEnemy ? enemyParent : allyParent;
        if (parent == null)
        {
            Debug.LogError("Spawn parent not assigned!");
            return;
        }

        foreach (var prefab in waveDef.spawnerPrefabs)
        {
            for (int i = 0; i < waveDef.unitsPerPrefab; i++)
            {
                Vector3 spawnPos = parent.position;

                // Optional designer offsets (still supported)
                if (waveDef.spawnOffsets != null && waveDef.spawnOffsets.Length > 0)
                {
                    spawnPos += waveDef.spawnOffsets[i % waveDef.spawnOffsets.Length];
                }

                // 🧠 Formation spacing
                spawnPos += GetFormationOffset(i, waveDef);


                PilotedVehicleSpawn spawner =
                    Instantiate(prefab, spawnPos, Quaternion.identity, parent);

                aliveUnits++;
                spawner.onDestroyed.AddListener(() => aliveUnits--);

                // ✅ Assign team to Trackable
                Trackable trackable = spawner.GetComponent<Trackable>();
                if (trackable != null)
                {
                    trackable.Team = waveDef.isEnemy ? enemyTeam : playerTeam;
                }

                // ✅ Assign AI role ONLY
                FighterAICombatBrain brain = spawner.GetComponent<FighterAICombatBrain>();
                if (brain != null)
                {
                    brain.combatRole = waveDef.combatRole;
                    brain.enemyTeam = waveDef.isEnemy ? playerTeam : enemyTeam;
                }
            }
        }
    }

    Vector3 GetFormationOffset(int index, UnitWaveDefinition wave)
{
    int row = index / wave.unitsPerRow;
    int col = index % wave.unitsPerRow;

    float xOffset = (col - (wave.unitsPerRow - 1) * 0.5f) * wave.horizontalSpacing;
    float yOffset = -row * wave.verticalSpacing;

    return new Vector3(xOffset, yOffset, 0);
}

}
*/