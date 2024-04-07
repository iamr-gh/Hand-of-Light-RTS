using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//ignores Move to Coordinate commands
public class SimpleEnemyAI : UnitAI
{

    protected override void Update()
    {
        if (target == null || (Vector3.Distance(target.transform.position, transform.position) >= parameters.getAggroRange())) {
            target = FindNextTarget();
        }

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        // If not null then target is the closest enemy in aggroRange
        if (target != null && Vector3.Distance(target.transform.position, transform.position) > parameters.getAttackRange()-0.25) { // REMOVED: targtetUnitObject.activeSelf check, maybe need to put it back in
            if (navAgent != null) {
                navAgent.SetDestination(target.transform.position);
            }
        }
        // Target is null, so no enemy within aggro range, stop moving
        else {
            if (navAgent != null) {
                navAgent.SetDestination(transform.position);
            }
        }
    }

    // Returns an enemy gameobject within aggro if one exists, otherwise returns null
    private GameObject FindNextTarget() {
        List<GameObject> enemies = new List<GameObject>(); // List of enemies within our aggro
        // Get all objects within our aggro range
        List<GameObject> potentialEnemies = GlobalUnitManager.singleton.FindNearby(transform.position, parameters.getAggroRange());
        foreach (GameObject obj in potentialEnemies)
        {
            UnitAffiliation otherAff = obj.GetComponent<UnitAffiliation>();
            UnitParameters otherUnitParameters = obj.GetComponent<UnitParameters>();
            //If the other object does not have parameters or damage handling, do nothing
            if (otherAff == null || otherUnitParameters == null) { continue; }
            if (affiliation.affiliation != otherAff.affiliation) { enemies.Add(obj); }
        }

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
