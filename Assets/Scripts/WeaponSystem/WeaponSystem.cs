using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitParameters))]
[RequireComponent(typeof(UnitAI))]
public abstract class WeaponSystem : MonoBehaviour
{
    protected UnitParameters parameters;
    public bool isAttacking = false;
    public bool offCooldown = true;
    protected UnitAI unitAI;
    protected GameObject target;

    // Start is called before the first frame update
    private void Start()
    {
        unitAI = GetComponent<UnitAI>();
        parameters = GetComponent<UnitParameters>();
        isAttacking = false;
        offCooldown = true;
    }

    private void Update()
    {
        // Get the target, then if not attacking and in range attack
        target = unitAI.getTarget();

        //offcooldown should never be true if isAttacking is true
        if (TargetInRange() && offCooldown && !isAttacking)
        {
            //Debug.Log("weapon system");
            //Debug.Log(target);
            isAttacking = true;
            offCooldown = false;
            StartCoroutine(ResetAttackCooldown());
            StartCoroutine(ResetAttackDuration());
            StartCoroutine(Attack());
        }
    }

    protected IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(parameters.getAttackCooldown());
        offCooldown = true;
    }

    protected IEnumerator ResetAttackDuration()
    {
        yield return new WaitForSeconds(parameters.getAttackDuration());
        isAttacking = false;
    }

    public bool TargetInRange()
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) <= parameters.getAttackRange()) { return true; }
        else { return false; }
    }

    //may not strictly need to be a prefab, but allowing ordered action on attack rn
    protected abstract IEnumerator Attack();

    public bool getAttackState() { return isAttacking; }

    // Update is called once per frame
    private void LateUpdate()
    {
        // Destroy dead unit
        // this should probably be moved
        if (parameters.getHP() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
