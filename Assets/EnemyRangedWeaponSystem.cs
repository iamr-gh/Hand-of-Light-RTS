using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedWeaponSystem : MonoBehaviour
{
    public GameObject projectile;
    public float delayTillDmg;
    private UnitParameters parameters;
    private Planner planner;
    private bool isAttacking = false;
    private UnitAI unitAI;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out unitAI);
        TryGetComponent(out parameters);
        TryGetComponent(out planner);
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

    private bool TargetInRange()
    {
        if (Vector3.Distance(target.transform.position, transform.position) <= parameters.getAttackRange()) { return true; }
        else { return false; }
    }

    private IEnumerator Attack()
    {
        //if moving then break
        // if (isAttacking) { yield break; } // If already attacking then return
        // isAttacking = true;


        // Deal Damage
        if (projectile != null)
        {
            var proj = Instantiate(projectile, transform.position, Quaternion.identity);
            if (proj.TryGetComponent(out FlyToTarget fly))
            {
                fly.target = target;
            }
        }

        StartCoroutine(DoDmg(target));
        yield return null;
        /*StartCoroutine(AttackFeedback()); // TODO: REMOVE THIS*/

        // yield return new WaitForSeconds(1.0f / parameters.getAttackRate());
        // isAttacking = false;
    }
    private IEnumerator DoDmg(GameObject target)
    {
        yield return new WaitForSeconds(delayTillDmg);
        if (target == null)
        {
            yield break;
        }
        UnitInteractions targetInteractions = target.GetComponent<UnitInteractions>();
        if (targetInteractions != null)
        {
            targetInteractions.TakeDamage(parameters.getAttackDamage());
        }
/*        UnitParameters targetParameters = target.GetComponent<UnitParameters>();
        float newHP = targetParameters.getHP() - parameters.getAttackDamage();
        targetParameters.setHP(newHP);*/
    }

    // TODO REMOVE THIS
    IEnumerator AttackFeedback()
    {
        AudioClip damageSound = Resources.Load<AudioClip>("Audio/DamageSound");
        Transform targetTransform = target.transform;
        SpriteRenderer otherSpriteRenderer = null;

        for (int childIdx = 0; childIdx < targetTransform.childCount; childIdx++)
        {
            GameObject child = targetTransform.GetChild(childIdx).gameObject;
            if (child.name == "RelativeRenderer")
            {
                otherSpriteRenderer = child.GetComponent<SpriteRenderer>();
                /*if(otherSpriteRenderer.color != Color.red) { 
                    originalOtherColor = otherSpriteRenderer.color;
                }*/
            }
        }

        AudioSource.PlayClipAtPoint(damageSound, Camera.main.transform.position);
        // UI JUICE STUFF TODO: MOVE TO AN EVENT-BASED SYSTEM
        float damageFlashPeriod = 0.1f; // Seconds
        if (otherSpriteRenderer != null) { otherSpriteRenderer.color = Color.red; }
        yield return new WaitForSeconds(damageFlashPeriod);
        if (otherSpriteRenderer != null) { otherSpriteRenderer.color = Color.white; }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Destroy dead unit
        // this should probably be moved
        if (parameters.getHP() <= 0)
        {
            Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}
