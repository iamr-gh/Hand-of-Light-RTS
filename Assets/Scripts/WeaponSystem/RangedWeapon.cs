using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponSystem
{
    public GameObject projectile;
    public float delayTillDmg;

    protected override IEnumerator Attack()
    {
        if (isAttacking) { yield break; } // If already attacking then return
        isAttacking = true;

        // Create projectile
        if (projectile != null)
        {
            var proj = Instantiate(projectile, transform.position, Quaternion.identity);
            if (proj.TryGetComponent(out FlyToTarget fly)) {
                // Set target and damage of projectile
                fly.target = target;
                fly.projectileDamage = parameters.getAttackDamage();
            }
        }

        yield return new WaitForSeconds(1.0f / parameters.getAttackRate());
        isAttacking = false;
    }
}
