using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitParameters))]

public class DamageSystem : MonoBehaviour
{
    public bool canDealDamage = true;
    public int teamID = 1;

    private UnitParameters parameters;
    private float attackCooldown = 0; // TODO: delete
    private bool isAttacking = false;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        parameters = GetComponent<UnitParameters>();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        if(target == null) { AcquireTarget(); }
        if(target != null && TargetInRange() && !isAttacking)
        {
            StartCoroutine(DoAttack());
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{

    //    GameObject objectCollidedWith = collision.gameObject;
    //    DamageSystem otherDamageSystem = objectCollidedWith.GetComponent<DamageSystem>();
    //    UnitParameters otherUnitParameters = objectCollidedWith.GetComponent<UnitParameters>();
    //    // If the other object does not have parameters or damage handling, do nothing
    //    if (otherDamageSystem == null || otherUnitParameters == null) { return; }
    //    Debug.Log("Collision: " + teamID + "-to-" + otherDamageSystem.teamID);

    //    // This object can deal damage and collided with an object of another team
    //    if (attackCooldown <= 0 && teamID != otherDamageSystem.teamID && canDealDamage)
    //    {
    //        // Deal Damage
    //        float newHP = otherUnitParameters.getHP() - parameters.getAttackDamage();
    //        otherUnitParameters.setHP(newHP);
    //        attackCooldown = 0.75f;
    //    }
    //}

    bool TargetInRange() {
        if((target.transform.position - transform.position).magnitude <= parameters.getAttackRange()) { return true; }
        else { return false; }
    }

    List<GameObject> FindEnemiesNearby()
    {
        List<GameObject> enemies = new List<GameObject>();
        List<GameObject> potentialEnemies = GlobalUnitManager.singleton.FindNearby(transform.position, parameters.getAttackRange());
        foreach(GameObject obj in potentialEnemies) {
            DamageSystem otherDamageSystem = obj.GetComponent<DamageSystem>();
            UnitParameters otherUnitParameters = obj.GetComponent<UnitParameters>();
            //If the other object does not have parameters or damage handling, do nothing
            if (otherDamageSystem == null || otherUnitParameters == null) { continue; }
            if (teamID != otherDamageSystem.teamID && canDealDamage) { enemies.Add(obj); }
        }
        return enemies;
    }

    void AcquireTarget()
    {
        List<GameObject> potentialTargets = FindEnemiesNearby();
        // TODO: find the one "in front" of the unit, use a dot product with the dot((enemyPos - pos), this->normalVector), use the enemy closest to 1
        if (potentialTargets.Count != 0) { target = potentialTargets[0]; }
        else { target = null; }
    }

    IEnumerator DoAttack() {
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
        if(parameters.getHP() <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
