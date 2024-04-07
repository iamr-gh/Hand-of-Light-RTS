using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class RecallAbility : Ability
{
    public float delay = 1.0f;
    private Dictionary<GameObject, Color> unitColorMap = new();
    RecallAbility() : base()
    {
        abilityName = "Recall";
        abilitySlot = 1;
        cooldown = 2;
        type = AbilityTypes.GroundTargetedAOE;
        aoeRadius = 3;
    }

    public override void OnCast(AbilityCastData castData)
    {
        // let's teleport all units back to the caster
        foreach (GameObject unit in castData.friendlyUnitsHit)
        {
            if (unit != castData.caster) {
                StartCoroutine(TeleportUnit(unit, castData.caster.transform.position + new Vector3(castData.targetPosition.x - unit.transform.position.x, 0, castData.targetPosition.z - unit.transform.position.z)));
            }
        }

        foreach (GameObject unit in castData.enemyUnitsHit)
        {
            StartCoroutine(TeleportUnit(unit, castData.caster.transform.position + new Vector3(castData.targetPosition.x - unit.transform.position.x, 0, castData.targetPosition.z - unit.transform.position.z)));
        }
    }

    private IEnumerator TeleportUnit(GameObject unit, Vector3 newLoc)
    {
        //add some juice
        // set halo component to E64BD7
        // nope we can't do that for some reason?

        //if we can, do sprite lerp to black
        if (unit.transform.childCount > 2 && unit.transform.GetChild(2).TryGetComponent(out SpriteRenderer sr))
        {
            // sr.color = Color.black;
            var start = sr.color;
            unitColorMap[unit] = start;

            var end = Color.black;
            //lerp over duration
            var elapsedTime = 0.0f;
            while (elapsedTime < delay)
            {
                sr.color = Color.Lerp(start, end, elapsedTime / delay);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //need to reset upon death 

            if (unit != null)
            {
                unit.transform.position = newLoc;
                if (unit.TryGetComponent(out UnitAI ai))
                {
                    ai.AttackMoveToCoordinate(newLoc);
                }
            }
            sr.color = start;
        }
        else
        {
            //naive, if something went wrong or object of different format
            yield return new WaitForSeconds(delay);
            if (unit != null)
            {
                unit.transform.position = newLoc;
            }
        }

    }
    void OnDestroy()
    {
        Debug.Log("Destroying Recall Ability Resetting:" + unitColorMap.Count.ToString() + " sprites");
        // reset color 
        foreach (var (unit, color) in unitColorMap)
        {
            if (unit != null && unit.transform.childCount > 2
                && unit.transform.GetChild(2).TryGetComponent(out SpriteRenderer sr))
            {
                sr.color = color;
            }
        }
    }
}
