using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(UnitAffiliation))]
/*[RequireComponent(typeof(Planner))]*/
[RequireComponent(typeof (NavMeshAgent))]
[RequireComponent(typeof(UnitParameters))]
public class UnitAI : MonoBehaviour
{
    //unit will monitor the behavior of one other agent, for the purpose of following/attack
    
    protected GameObject target = null;
    protected UnitParameters parameters;
    protected UnitAffiliation affiliation;
    protected NavMeshAgent navAgent;
    protected WeaponSystem weaponSystem;

    protected virtual void Start()
    {
        TryGetComponent(out affiliation);
        TryGetComponent(out parameters);
        TryGetComponent(out navAgent);
        TryGetComponent(out weaponSystem);
        navAgent.speed = parameters.getMovementSpeed();
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
            // planner.changeWayPointXZ(new Vector2(coord.x, coord.z));
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
            // planner.changeWayPointXZ(new Vector2(coord.x, coord.z));
        }
        //common extension will be move then attack once within a certain range
    }
    
    public virtual void Stop(){
        
    }

    public virtual void setTarget(GameObject tgt){
        target = tgt;
    }

    public virtual GameObject getTarget(){
        return target;
    }
}
