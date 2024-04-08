using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponSystem
{
    protected override IEnumerator Attack() {

        // Deal Damage
        WeaponSystem weaponSystem = target.GetComponent<WeaponSystem>();
        //UnitInteractions targetInteractions = target.GetComponent<UnitInteractions>();
        if (weaponSystem.juice != null) {
            StartCoroutine(juice.AttackJuice());
            weaponSystem.juice.TakeDamage(parameters.getAttackDamage());
        }
        yield return null;
    }
}
