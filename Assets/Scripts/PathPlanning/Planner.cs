using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Planner : MonoBehaviour
{
    //public interface for all kinds of planners
    public Vector2 goal;
    
    public float maxvel;

    public UnityEvent reachedGoalEvent;

    public UnityEvent finishedAttackingEvent; // unimplemented

    protected bool reachedGoal = true;
    
    protected Rigidbody rb;
    //spawn units to stay where they are
    protected virtual void Start(){
        goal = new Vector2(transform.position.x,transform.position.z);
        // Debug.Log("Initial goal setup");
        // Debug.Log(goal);
        TryGetComponent(out rb);
    }
    public void changeWayPointXZ(Vector2 newGoal){
        //at some point might make more sense as a message passing system 
        goal = newGoal;
        reachedGoal = false;
        // Debug.Log("Set new goal");
        // Debug.Log(goal);
    }
    
}
