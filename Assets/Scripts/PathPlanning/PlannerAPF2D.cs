using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlannerAPF2D : MonoBehaviour
{
    //eventually I would like to decouple from rb
    private Rigidbody rb;
    //will need to put vel constraints, make a util lib
    public Vector2 goal;
    public float goal_weight = 1;
    public float obav_weight = 1;
    public float maxvel;
    public float obav_radius = 4;
    void Start()
    {
        TryGetComponent(out rb);
    }

    // Update is called once per frame
    void Update()
    {
        //movement in XZ
        Vector2 pos_2d = new Vector2(transform.position.x,transform.position.z);
        var goal_offset = goal-pos_2d;
        //more components of APF
        
        //obav, inv square of distance
        var obav_offset = Vector2.zero;
        foreach(GameObject obj in GlobalUnitManager.singleton.FindNearby(transform.position,obav_radius)){
            if(TryGetComponent(out UnitAffiliation unitaff)){
                //ignore unts in this rn
                continue;
            }
            
            var offset = transform.position - obj.transform.position;
            var offset2d = new Vector2(offset.x,offset.z); //this transform is annoying

            obav_offset += (1/offset2d.sqrMagnitude) * offset2d.normalized;
        }


        
        //will need weights fs
        var overall = goal_weight*goal_offset + obav_weight*obav_offset;
        
        Vector3 next_vel = new Vector3(overall.x, 0, overall.y);
        next_vel = Vector3.ClampMagnitude(next_vel,maxvel);
        rb.velocity = next_vel;
    }
}
