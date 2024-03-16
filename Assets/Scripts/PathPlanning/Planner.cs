using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Planner : MonoBehaviour
{
    //public interface for all kinds of planners
    public Vector2 goal;
    
    protected Rigidbody rb;
    //spawn units to stay where they are
    protected virtual void Start(){
        goal = new Vector2(transform.position.x,transform.position.z);
        Debug.Log("Initial goal setup");
        Debug.Log(goal);
        TryGetComponent(out rb);
    }
    public void changeWayPointXZ(Vector2 newGoal){
        //at some point might make more sense as a message passing system 
        goal = newGoal;
        Debug.Log("Set new goal");
        Debug.Log(goal);
    }
    
}
