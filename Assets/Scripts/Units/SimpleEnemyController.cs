using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyController : MonoBehaviour
{
    private UnitParameters parameters;

    private List<GameObject> nearbyUnits;
    private bool hasAggro = false;
    /* FIXME: reference to gameobject - remove once damage system destroys gameobject instead of setting to inactive*/
    private GameObject targtetUnitObject;
    private Transform targetUnit;
    private float distanceToTarget;

    PlannerAPF2D planner;

    private void Start()
    {
        parameters = GetComponent<UnitParameters>();
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
        nearbyUnits = GlobalUnitManager.singleton.FindNearby(transform.position, parameters.getAggroRange());
        for (int i = 0; i < nearbyUnits.Count; i++)
        {
            if (nearbyUnits[i] != null && nearbyUnits[i].TryGetComponent(out UnitAffiliation unitaff))
            {
                if(nearbyUnits[i].gameObject.activeSelf && unitaff.affiliation != "Red") {
                    targetUnit = nearbyUnits[i].gameObject.transform;
                    targtetUnitObject = nearbyUnits[i].gameObject;
                    hasAggro = true;
                    break;
                }
            }
        }
    }

    private void MoveToTarget()
    {
        distanceToTarget = Vector3.Distance(targetUnit.position, transform.position);
        if(targtetUnitObject.activeSelf && distanceToTarget <= parameters.getAggroRange())
        {
            planner.changeWayPointXZ(new Vector2(targetUnit.position.x, targetUnit.position.z));
        }
        else
        {
            planner.changeWayPointXZ(new Vector2(transform.position.x, transform.position.z));
            hasAggro = false;
        }
    }
}
