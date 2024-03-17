using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitParameters))]

public class DamageSystem : MonoBehaviour
{
    public bool canDealDamage = true;
    public int teamID = 1;

    private UnitParameters parameters;

    // Start is called before the first frame update
    void Start()
    {
        parameters = GetComponent<UnitParameters>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject objectCollidedWith = collision.gameObject;
        DamageSystem otherDamageSystem = objectCollidedWith.GetComponent<DamageSystem>();
        UnitParameters otherUnitParameters = objectCollidedWith.GetComponent<UnitParameters>();
        // If the other object does not have parameters or damage handling, do nothing
        if (otherDamageSystem == null || otherUnitParameters == null) { return; }
        Debug.Log("Collision: " + teamID + "-to-" + otherDamageSystem.teamID);

        // This object can deal damage and collided with an object of another team
        if (teamID != otherDamageSystem.teamID && canDealDamage)
        {
            // Deal Damage
            float newHP = otherUnitParameters.getHP() - parameters.getAttackDamage();
            otherUnitParameters.setHP(newHP);
        }
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
