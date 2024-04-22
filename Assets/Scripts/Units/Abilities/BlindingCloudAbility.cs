using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlindingCloudAbility : Ability
{
    public GameObject cloudPrefab;
    public GameObject cloudVfx;
    public float cast_delay = 0.5f;
    public float cloud_duration = 5.0f;
    public float cloud_height = 0.5f;
    
    // Start is called before the first frame update
    
    // void  Start()
    // {
    //     // cloudPrefab = Resources.Load<GameObject>("Prefabs/BlindingCloud");
        
    // }
    
    BlindingCloudAbility() : base()
    {
        abilityName = "Smoke Cloud";
        abilitySlot = 1;
        cooldown = 2;
        type = AbilityTypes.GroundTargetedAOE;
        aoeRadius = 4;
    }

    public override void OnCast(AbilityCastData castData) {
        // spawn a cloud at the target location after a delay
        var blinding_cloud = Instantiate(cloudPrefab, castData.targetPosition + cloud_height*Vector3.up, Quaternion.identity);
        var vfx = Instantiate(cloudVfx, blinding_cloud.transform);
        //scale with radius
        blinding_cloud.transform.localScale = new Vector3(aoeRadius * 2, blinding_cloud.transform.localScale.y, aoeRadius * 2);
        vfx.transform.localScale = new Vector3(aoeRadius * 2, vfx.transform.localScale.y, aoeRadius * 2);
        if(blinding_cloud.TryGetComponent(out ExpireWithTime expire)){
            expire.timeToLive = cloud_duration;
        }
        
    }

}
