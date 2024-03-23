using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitParameters))]
[RequireComponent(typeof(UnitAI))]
public class WeaponSystem : MonoBehaviour
{
    public bool canDealDamage = true; // TODO: delete this, then fix dependency in AttractToWeakEnemyController

    private UnitParameters parameters;
    private bool isAttacking = false;
    private UnitAI unitAI;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        unitAI = GetComponent<UnitAI>();
        parameters = GetComponent<UnitParameters>();
    }

    private void Update()
    {
        
        // Get the target, then if not attacking and in range attack
        target = unitAI.getTarget();
        if (target != null && TargetInRange() && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private bool TargetInRange() {
        if(Vector3.Distance(target.transform.position, transform.position) <= parameters.getAttackRange()) { return true; }
        else { return false; }
    }

    private IEnumerator Attack() {
        if(isAttacking) { yield break; } // If already attacking then return

        isAttacking = true;
        // Deal Damage
        UnitParameters targetParameters = target.GetComponent<UnitParameters>();
        float newHP = targetParameters.getHP() - parameters.getAttackDamage();
        targetParameters.setHP(newHP);

        yield return new WaitForSeconds(1.0f / parameters.getAttackRate());
        isAttacking = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Destroy dead unit
        // this should probably be moved
        if(parameters.getHP() <= 0)
        {
            Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}
