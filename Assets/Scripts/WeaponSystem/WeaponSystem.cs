using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitParameters))]
[RequireComponent(typeof(UnitAI))]
public abstract class WeaponSystem : MonoBehaviour
{
    protected UnitParameters parameters;
    protected bool isAttacking = false;
    protected UnitAI unitAI;
    protected GameObject target;

    // Start is called before the first frame update
    private void Start()
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
            //Debug.Log("weapon system");
            //Debug.Log(target);
            StartCoroutine(Attack());
        }
    }

    private bool TargetInRange() {
        if(Vector3.Distance(target.transform.position, transform.position) <= parameters.getAttackRange()) { return true; }
        else { return false; }
    }

    protected abstract IEnumerator Attack();

    public bool getAttackState() { return isAttacking; }

    // Update is called once per frame
    private void LateUpdate()
    {
        // Destroy dead unit
        // this should probably be moved
        if(parameters.getHP() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
