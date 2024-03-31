using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitAffiliation))]
/*[RequireComponent(typeof(Planner))]*/
[RequireComponent(typeof(UnitParameters))]
public class UnitAI : MonoBehaviour
{
    //unit will monitor the behavior of one other agent, for the purpose of following/attack
    protected GameObject target = null;
    protected UnitParameters parameters;
    protected Planner planner;
    protected UnitAffiliation affiliation;
    protected NavMeshAgent navAgent;

    protected virtual void Start()
    {
        TryGetComponent(out affiliation);
        TryGetComponent(out parameters);
        TryGetComponent(out planner);
        TryGetComponent(out navAgent);
/*        planner.maxvel = parameters.getMovementSpeed();*/
    }
    
    protected virtual void Update(){
        
    }
    //create a standard set of commands or interfaces
    // may add a unit mode/state in the future
    // public virtual void AttackTarget(GameObject trg){
    //     target = trg;
    // }
    
    public virtual void MoveToCoordinate(Vector3 coord){
        if(navAgent != null) {
            navAgent.SetDestination(coord);
        }
        else {
            Debug.Log("Moving with planner");
            planner.changeWayPointXZ(new Vector2(coord.x, coord.z));
        }
        //common extension will be move then attack once within a certain range
    }

    public virtual void AttackMoveToCoordinate(Vector3 coord) {
        NavMeshAgent navAgent;
        TryGetComponent(out navAgent);
        if (navAgent != null)
        {
            navAgent.SetDestination(coord);
        }
        else
        {
            Debug.Log("Moving with planner");
            planner.changeWayPointXZ(new Vector2(coord.x, coord.z));
        }
        //common extension will be move then attack once within a certain range
    }

    public virtual void setTarget(GameObject tgt){
        target = tgt;
    }

    public virtual GameObject getTarget(){
        return target;
    }
}
