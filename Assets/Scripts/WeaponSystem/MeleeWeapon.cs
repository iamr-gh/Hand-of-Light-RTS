using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponSystem
{
    protected override IEnumerator Attack() {
        // if (isAttacking) { yield break; } // If already attacking then return
        // isAttacking = true;

        // Deal Damage
        UnitInteractions targetInteractions = target.GetComponent<UnitInteractions>();
        if (targetInteractions != null) {
            targetInteractions.TakeDamage(parameters.getAttackDamage());
        }
        yield return null;
    }
}
