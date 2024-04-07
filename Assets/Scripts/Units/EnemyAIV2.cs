using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIV2 : UnitAIV2
{
    public float social_radius = 3.0f; //neighbors you can see
    public float change_tolerance = 0.5f;
    Vector3 last_nearby_centroid;

    //all the fancy logic from user controlled agents(probably less nessecary)
    protected override void Start()
    {
        base.Start();
        // StopAllCoroutines();
        // StartCoroutine(base.WeaponSystemManager());
        last_nearby_centroid = calcCentroid();
        //NO ANTI JITTER CHECKER
        // stopped = false;
    }

    protected override void Update()
    {
        base.Update();
        //if not attacking, and neighbors move, follow them a bit
        if (target == null && attacking)
        {
            var next_centroid = calcCentroid();
            var change = next_centroid - last_nearby_centroid;
            if (change.magnitude > change_tolerance)
            {
                // last_nearby_centroid = next_centroid;
                // this can create a weird momentum
                AttackMoveToCoordinate(transform.position + change);
                last_nearby_centroid = next_centroid;
            }
        }

    }

    private Vector3 calcCentroid()
    {
        var neighbors = GlobalUnitManager.singleton.FindNearby(transform.position, social_radius);
        // get centroid of neighbors
        Vector3 centroid = Vector3.zero;

        //purposefully include self
        foreach (GameObject obj in neighbors)
        {
            if(obj != null){
                centroid += obj.transform.position;
            }
        }

        centroid /= neighbors.Count;
        return centroid;

    }

}
