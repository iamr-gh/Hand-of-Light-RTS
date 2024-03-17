using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyController : MonoBehaviour
{
    public float aggroRange;
    public float attackRange;

    private List<GameObject> nearbyUnits;
    public bool hasAggro = false;
    private Transform targetUnit;
    private float distanceToTarget;

    PlannerAPF2D planner;

    private void Start()
    {
        planner = GetComponent<PlannerAPF2D>();
    }

    private void Update()
    {
        if (hasAggro)
        {
            MoveToTarget();
        }
        else
        {
            CheckForTargets();
        }
    }

    private void CheckForTargets()
    {
        nearbyUnits = GlobalUnitManager.singleton.FindNearby(transform.position, aggroRange);
        for (int i = 0; i < nearbyUnits.Count; i++)
        {
            if (nearbyUnits[i] != null && nearbyUnits[i].TryGetComponent(out UnitAffiliation unitaff))
            {
                if(unitaff.affiliation != "Red") {
                    targetUnit = nearbyUnits[i].gameObject.transform;
                    hasAggro = true;
                    break;
                }
            }
        }
    }

    private void MoveToTarget()
    {
        distanceToTarget = Vector3.Distance(targetUnit.position, transform.position);
        if(distanceToTarget <= aggroRange)
        {
            planner.changeWayPointXZ(new Vector2(targetUnit.position.x, targetUnit.position.z));
        }
        else
        {
            hasAggro = false;
        }
    }
}
