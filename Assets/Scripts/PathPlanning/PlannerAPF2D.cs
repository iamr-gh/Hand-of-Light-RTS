using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(UnitAffiliation))]
public class PlannerAPF2D : Planner
{
    //eventually I would like to decouple from rb
    //will need to put vel constraints, make a util lib
    public float goal_weight = 1;
    public float obav_weight = 1;
    public float friendly_av_weight = 1;
    public float enemy_av_weight = 0;
    //the degree which above is automated is kind of strange
    public float obav_radius = 4;

    private UnitAffiliation unitAffiliation;

    protected override void Start()
    {
        base.Start();
        TryGetComponent(out unitAffiliation);
    }

    // Update is called once per frame
    void Update()
    {
        //movement in XZ
        Vector2 pos_2d = new Vector2(transform.position.x, transform.position.z);
        var goal_offset = goal - pos_2d;
        //more components of APF

        //obav, inv square of distance
        var obav_offset = Vector2.zero;
        var friendly_av_offset = Vector2.zero;
        var enemy_av_offset = Vector2.zero;

        //ignoring any enemy av weight

        foreach (GameObject obj in GlobalUnitManager.singleton.FindNearby(transform.position, obav_radius))
        {
            if(obj == gameObject){
                continue;
            }
            var offset = transform.position - obj.transform.position;
            var offset2d = new Vector2(offset.x, offset.z); //this transform is annoying
            if (TryGetComponent(out UnitAffiliation unitaff))
            {
                //ignore unts in this rn
                if (unitaff.affiliation == unitAffiliation.affiliation)
                {
                    friendly_av_offset += (1 / offset2d.sqrMagnitude) * offset2d.normalized;
                }
                else
                {
                    enemy_av_offset += (1 / offset2d.sqrMagnitude) * offset2d.normalized;
                }
            }
            else{
                obav_offset += (1 / offset2d.sqrMagnitude) * offset2d.normalized;
            }


        }



        //weight different priorities
        var overall = goal_weight * goal_offset;
        overall += obav_weight * obav_offset;
        overall += friendly_av_weight * friendly_av_offset;
        overall += enemy_av_weight * enemy_av_offset;

        Vector3 next_vel = new Vector3(overall.x, 0, overall.y);
        next_vel = Vector3.ClampMagnitude(next_vel, maxvel);
        rb.velocity = next_vel;

        if (!reachedGoal && Mathf.Approximately(next_vel.magnitude, 0)) {
            reachedGoalEvent.Invoke();
            reachedGoal = true;
        }
    }
    void OnDisable() {
        if (!reachedGoal) {
            reachedGoalEvent.Invoke();
            reachedGoal = true;
        }
    }

}
