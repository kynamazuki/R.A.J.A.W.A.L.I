/*using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

[CreateAssetMenu(fileName = "UnitWaveDefinition", menuName = "Game/UnitWave")]
public class UnitWaveDefinition : ScriptableObject
{
    public string waveName;

    [Header("Spawn")]
    public List<PilotedVehicleSpawn> spawnerPrefabs;

    [Min(1)]
    public int unitsPerPrefab = 3;

    public Vector3[] spawnOffsets;

    [Header("Formation")]
    public float horizontalSpacing = 40f;
    public float verticalSpacing = 25f;
    public int unitsPerRow = 2;


    [Header("Combat")]
    [Tooltip("Air: fighters first | Strike: capitals first | Defend: protect allies")]
    public CombatRole combatRole;

    public bool isEnemy = true;

    [Header("Timing")]
    public float waveInterval = 5f;
}
*/