using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

[RequireComponent(typeof(UnitAffiliation))]
[RequireComponent(typeof(Planner))]
[RequireComponent(typeof(UnitParameters))]
public class UnitAI : MonoBehaviour
{
    //unit will monitor the behavior of one other agent, for the purpose of following/attack
    protected GameObject target = null;
    protected UnitParameters parameters;
    protected Planner planner;
    protected UnitAffiliation affiliation;

    protected virtual void Start()
    {
        TryGetComponent(out affiliation);
        TryGetComponent(out parameters);
        TryGetComponent(out planner);
    }
    
    protected virtual void Update(){
        
    }
    //create a standard set of commands or interfaces
    // may add a unit mode/state in the future
    // public virtual void AttackTarget(GameObject trg){
    //     target = trg;
    // }
    
    public virtual void MoveToCoordinate(Vector2 coord){
        planner.changeWayPointXZ(coord);
        //common extension will be move then attack once within a certain range
    }
    
    public virtual void setTarget(GameObject tgt){
        target = tgt;
    }

    public virtual GameObject getTarget(){
        return target;
    }
}
