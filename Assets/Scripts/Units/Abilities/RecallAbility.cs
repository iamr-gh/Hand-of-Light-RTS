using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class RecallAbility : Ability {
    public float delay = 1.0f;
    public GameObject circlePrefab;

    public GameObject summoningVfx;

    private Dictionary<GameObject, Color> unitColorMap = new();
    RecallAbility() : base() {
        abilityName = "Recall";
        abilitySlot = 1;
        cooldown = 2;
        type = AbilityTypes.GroundTargetedAOE;
        aoeRadius = 3;
    }

    public override void OnCast(AbilityCastData castData) {
        StartCoroutine(CastCoroutine(castData));
    }

    IEnumerator CastCoroutine(AbilityCastData castData) {
        var circle = Instantiate(circlePrefab, castData.targetPosition, Quaternion.identity);
        circle.transform.localScale = new Vector3(aoeRadius * 2, circle.transform.localScale.y, aoeRadius * 2);
        // let's teleport all units back to the caster
        foreach (GameObject unit in castData.friendlyUnitsHit) {
            if (unit != castData.caster) {
                StartCoroutine(TeleportUnit(unit, castData.caster.transform.position + new Vector3(castData.targetPosition.x - unit.transform.position.x, 0, castData.targetPosition.z - unit.transform.position.z)));
            }
        }

        foreach (GameObject unit in castData.enemyUnitsHit) {
            StartCoroutine(TeleportUnit(unit, castData.caster.transform.position + new Vector3(castData.targetPosition.x - unit.transform.position.x, 0, castData.targetPosition.z - unit.transform.position.z)));
        }
        yield return new WaitForSeconds(delay);
        Destroy(circle);
    }

    private IEnumerator TeleportUnit(GameObject unit, Vector3 newLoc) {
        //add some juice
        // set halo component to E64BD7
        // nope we can't do that for some reason?
        var vfx = Instantiate(summoningVfx, unit.transform);
        //if we can, do sprite lerp to black
        if (unit.transform.childCount > 2 && unit.transform.GetChild(2).TryGetComponent(out SpriteRenderer sr)) {
            // sr.color = Color.black;
            var start = sr.color;
            unitColorMap[unit] = start;

            var end = Color.black;
            //lerp over duration
            var elapsedTime = 0.0f;
            while (elapsedTime < delay) {
                if (sr != null) {
                    sr.color = Color.Lerp(start, end, elapsedTime / delay);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //need to reset upon death 

            if (unit != null) {
                unit.GetComponent<NavMeshAgent>().enabled = false;
                // unit.GetComponent<Rigidbody>().enab = false;
                unit.transform.position = newLoc;
                //we get some small null errors, let's see if breaks in build
                yield return null;
                unit.GetComponent<NavMeshAgent>().enabled = true;
                yield return null;
                if (unit.TryGetComponent(out UnitAI ai)) {
                    ai.AttackMoveToCoordinate(newLoc);
                }
            }
            sr.color = start;
        } else {
            //naive, if something went wrong or object of different format
            yield return new WaitForSeconds(delay);
            if (unit != null) {
                unit.transform.position = newLoc;
            }
        }
        Destroy(vfx);
    }
    void OnDestroy() {
        Debug.Log("Destroying Recall Ability Resetting:" + unitColorMap.Count.ToString() + " sprites");
        // reset color 
        foreach (var (unit, color) in unitColorMap) {
            if (unit != null && unit.transform.childCount > 2
                && unit.transform.GetChild(2).TryGetComponent(out SpriteRenderer sr)) {
                sr.color = color;
            }
        }
    }
}
