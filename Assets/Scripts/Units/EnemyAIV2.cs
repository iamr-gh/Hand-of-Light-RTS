using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class EnemyAIV2 : UnitAIV2
{
    //centroid method is bad, let's just let them communicate target
    public float social_radius = 3.0f; //neighbors you can see
    public GameObject seenTarget = null;

    //all the fancy logic from user controlled agents(probably less nessecary)
    protected override void Start()
    {
        base.Start();
        // StopAllCoroutines();
        // StartCoroutine(base.WeaponSystemManager());
        // (last_nearby_centroid, last_num) = calcCentroid();
        //NO ANTI JITTER CHECKER
        // stopped = false;
    }

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            seenTarget = target;
        }

        //if not attacking, and neighbors have a target, follow them
        if (target == null && attacking)
        {
            var (centroid, has_target) = calcCentroid();
            if (has_target)
            {
                AttackMoveToCoordinate(centroid);
            }
        }

    }

    private (Vector3, bool) calcCentroid()
    {
        var neighbors = GlobalUnitManager.singleton.FindNearby(transform.position, social_radius);
        // get centroid of neighbors
        Vector3 centroid = Vector3.zero;

        bool seen = false;
        foreach (GameObject obj in neighbors)
        {
            if (obj == gameObject || obj.TryGetComponent(out EnemyAIV2 enemyAIV2) && enemyAIV2.seenTarget != null)
            {
                //could give a target to each other if we want, for more graph effects. This will be good enough for now tho
                if (obj != gameObject)
                    seen = true;
                centroid += obj.transform.position;
            }
        }

        centroid /= neighbors.Count;
        return (centroid, seen);
    }

}
