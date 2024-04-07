using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallAbility : Ability
{
    RecallAbility() : base() {
        type = AbilityTypes.GroundTargetedAOE;
        abilitySlot = 1;
        aoeRadius = 3;
    }

    public override void OnCast(AbilityCastData castData) {
        Debug.Log("Recalling " + castData.friendlyUnitsHit.Count + " friendly units, also hit " + castData.enemyUnitsHit.Count + " enemy units");
    }
}
