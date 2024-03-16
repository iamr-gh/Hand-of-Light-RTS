using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS JUST A TESTING FRAMEWORK, DO NOT USE FOR SOMETHING REAL
public class TESTER_Beacon : MonoBehaviour
{
    // Start is called before the first frame update
    public bool broadCasting = true;
    public Vector2 broadCastGoal;
    public string affiliation;
    // private Vector2 lastBroadcast;
    public GameObject[] receivers;
    void Start()
    {
        // lastBroadcast = broadCastGoal;
        receivers = GameObject.FindGameObjectsWithTag("Managed");
    }

    // Update is called once per frame
    void Update()
    {
        if(broadCasting){
            // lastBroadcast = broadCastGoal;
            foreach(GameObject obj in receivers){
                if(obj != null){
                    //check if has a unit aff
                    if(obj.TryGetComponent(out UnitAffiliation aff)){
                        //for now broadcast to all aff
                        if(aff.affiliation == affiliation){
                            if(obj.TryGetComponent(out Planner plan)){
                                plan.changeWayPointXZ(broadCastGoal);
                            }
                        }
                    }
                }
            }
            Debug.Log("Tried to send from beacon");
            broadCasting = false;
        }
    }
}
