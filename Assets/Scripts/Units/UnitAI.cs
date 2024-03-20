using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public int teamID = 1;
    
    /* FIXME: reference to gameobject - remove once damage system destroys gameobject instead of setting to inactive*/
    private GameObject target = null;
    private UnitParameters parameters;
    private PlannerAPF2D planner;

    private void Start()
    {
        parameters = GetComponent<UnitParameters>();
        planner = GetComponent<PlannerAPF2D>();
    }

    private void Update()
    {
        if (target == null || (Vector3.Distance(target.transform.position, transform.position) >= parameters.getAggroRange())) {
            target = FindNextTarget();
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        // If not null then target is the closest enemy in aggroRange
        if (target != null) { // REMOVED: targtetUnitObject.activeSelf check, maybe need to put it back in
            planner.changeWayPointXZ(new Vector2(target.transform.position.x, target.transform.position.z));
        }
        // Target is null, so no enemy within aggro range, stop moving
        else {
            planner.changeWayPointXZ(new Vector2(transform.position.x, transform.position.z));
        }
    }

    public GameObject getTarget() { return target; }

    // Returns an enemy gameobject within aggro if one exists, otherwise returns null
    private GameObject FindNextTarget() {
        List<GameObject> enemies = new List<GameObject>(); // List of enemies within our aggro
        // Get all objects within our aggro range
        List<GameObject> potentialEnemies = GlobalUnitManager.singleton.FindNearby(transform.position, parameters.getAggroRange());
        foreach (GameObject obj in potentialEnemies)
        {
            UnitAI otherAI = obj.GetComponent<UnitAI>();
            UnitParameters otherUnitParameters = obj.GetComponent<UnitParameters>();
            //If the other object does not have parameters or damage handling, do nothing
            if (otherAI == null || otherUnitParameters == null) { continue; }
            if (teamID != otherAI.teamID) { enemies.Add(obj); }
        }

        // Find the closest enemy
        float minDistance = float.MaxValue;
        GameObject priorityEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float enemyDistance = (enemy.transform.position - transform.position).magnitude;
            if (enemyDistance < minDistance)
            {
                minDistance = enemyDistance;
                priorityEnemy = enemy;
            }
        }
        return priorityEnemy;
    }
}
