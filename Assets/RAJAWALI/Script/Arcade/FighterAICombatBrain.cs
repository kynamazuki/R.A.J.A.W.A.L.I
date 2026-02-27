/*using UnityEngine;
using System.Collections.Generic;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;

public enum CombatRole
{
    AirSuperiority, // Fighters first, then capitals
    Strike,         // Capitals first, then fighters
    Defend          // Protect a specific target
}

[RequireComponent(typeof(Tracker))]
[RequireComponent(typeof(Weapons))]
public class FighterAICombatBrain : MonoBehaviour
{
    [Header("Combat Role")]
    public CombatRole combatRole;

    [Header("Teams")]
    public Team enemyTeam; // Which team this AI considers as enemies
    public Team selfTeam;  // Its own team

    [Header("Vehicle Classes")]
    public VehicleClass fighterClass;
    public VehicleClass capitalClass;

    [Header("Trackable Types")]
    public TrackableType fighterTypeSO;
    public TrackableType capitalTypeSO;

    [Header("Defend Role")]
    public Transform defendedTarget;
    public float defendRadius = 300f;

    [Header("Retargeting")]
    [SerializeField] private float retargetInterval = 1f;

    private Tracker tracker;
    private Weapons weapons;
    private SpaceshipAttackBehaviour attackBehaviour;
    private TargetSelector targetSelector;

    private float retargetTimer;

    private void Awake()
    {
        tracker = GetComponent<Tracker>();
        weapons = GetComponent<Weapons>();
        attackBehaviour = GetComponent<SpaceshipAttackBehaviour>();
        targetSelector = weapons?.WeaponsTargetSelector;

        if (targetSelector != null)
        {
            targetSelector.SelectableTeams = new List<Team> { enemyTeam };
            targetSelector.scanEveryFrame = true; // Must be true to detect all trackables
        }
    }

    private void Update()
    {
        if (tracker == null || targetSelector == null || attackBehaviour == null)
            return;

        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            Trackable chosenTarget = SelectTargetByRole();

            if (chosenTarget != null)
            {
                // Dynamically set selectable type based on chosen target
                if (chosenTarget.GetComponentInParent<Vehicle>().VehicleClass == fighterClass)
                    targetSelector.SelectableTypes = new List<TrackableType> { fighterTypeSO };
                else
                    targetSelector.SelectableTypes = new List<TrackableType> { capitalTypeSO };
            }

            targetSelector.Select(chosenTarget);
            attackBehaviour.enabled = (chosenTarget != null);

            retargetTimer = retargetInterval;
        }
    }

    private Trackable SelectTargetByRole()
    {
        if (tracker.Targets == null || tracker.Targets.Count == 0)
            return null;

        Trackable chosen = null;

        switch (combatRole)
        {
            case CombatRole.AirSuperiority:
                // Pick nearest fighter first
                chosen = FindNearestTargetOfClass(fighterClass);
                if (chosen == null)
                    chosen = FindNearestTargetOfClass(capitalClass);
                break;

            case CombatRole.Strike:
                // Pick nearest capital first
                chosen = FindNearestTargetOfClass(capitalClass);
                if (chosen == null)
                    chosen = FindNearestTargetOfClass(fighterClass);
                break;

            case CombatRole.Defend:
                chosen = FindDefendTarget();
                break;
        }

        return chosen;
    }

    private Trackable FindNearestTargetOfClass(VehicleClass desiredClass)
    {
        Trackable nearest = null;
        float minDist = float.MaxValue;

        foreach (Trackable t in tracker.Targets)
        {
            if (t == null) continue;

            Vehicle v = t.GetComponentInParent<Vehicle>();
            if (v == null) continue;

            if (v.VehicleClass != desiredClass) continue;
            if (t.Team != enemyTeam) continue; // Only enemies

            float dist = Vector3.Distance(transform.position, t.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = t;
            }
        }

        return nearest;
    }

    private Trackable FindDefendTarget()
    {
        if (defendedTarget == null) return null;

        Trackable nearest = null;
        float minDist = float.MaxValue;

        foreach (Trackable t in tracker.Targets)
        {
            if (t == null) continue;

            Vehicle v = t.GetComponentInParent<Vehicle>();
            if (v == null) continue;

            // Only friendly fighters near defended target
            if (v.VehicleClass != fighterClass) continue;
            if (t.Team == enemyTeam) continue;

            float dist = Vector3.Distance(t.transform.position, defendedTarget.position);
            if (dist <= defendRadius && dist < minDist)
            {
                minDist = dist;
                nearest = t;
            }
        }

        return nearest;
    }
}
*/