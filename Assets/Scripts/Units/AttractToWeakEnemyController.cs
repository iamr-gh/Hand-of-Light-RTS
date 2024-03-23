using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractToWeakEnemyController : MonoBehaviour
{
    public float aggroRange;
    public float attackRange;

    private List<GameObject> nearbyUnits;
    public bool hasAggro = false;
    /*reference to gameobject - will delete once damage system destroys gameobject instead of setting to inactive*/
    private GameObject targtetUnitObject;
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
                if(nearbyUnits[i].gameObject.activeSelf && unitaff.affiliation != "Red") {
                    if(nearbyUnits[i].TryGetComponent(out WeaponSystem weaponSystem)){
                        //this entire component is dep
                        // if(!weaponSystem.canDealDamage){
                        //     targetUnit = nearbyUnits[i].gameObject.transform;
                        //     targtetUnitObject = nearbyUnits[i].gameObject;
                        //     hasAggro = true;
                        //     break;
                        // }
                    }
                }
            }
        }
    }

    private void MoveToTarget()
    {
        distanceToTarget = Vector3.Distance(targetUnit.position, transform.position);
        Debug.Log(distanceToTarget);
        if(targtetUnitObject.activeSelf && distanceToTarget <= aggroRange)
        {
            planner.changeWayPointXZ(new Vector2(targetUnit.position.x, targetUnit.position.z));
        }
        else
        {
            hasAggro = false;
        }
    }
}
