using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AbilityTypes {
    GroundTargetedAOE,
}

[System.Serializable]
public class AbilityCastData {
    public GameObject caster;
    public Vector3 targetPosition;
    public List<GameObject> friendlyUnitsHit;
    public List<GameObject> enemyUnitsHit;
}

public class Ability : MonoBehaviour
{
    public string abilityName;
    public Sprite abilityIcon;
    public int abilitySlot;
    public float cooldown;
    public AbilityTypes type;
    public float aoeRadius;

    private bool canCast = true;
    private float timeSinceCast;

    public virtual void OnCast(AbilityCastData castData) {

    }

    public bool CanCast() {
        return canCast;
    }

    public float GetCooldownProgress() {
        return timeSinceCast / cooldown;
    }

    public void OnCastWrapper(AbilityCastData castData) {
        if (canCast) {
            canCast = false;
            OnCast(castData);
            StartCoroutine(Cooldown());
        }
    }

    private void Start() {
        timeSinceCast = cooldown;
    }

    private IEnumerator Cooldown() {
        timeSinceCast = 0;
        while (timeSinceCast < cooldown) {
            yield return null;
            timeSinceCast += Time.deltaTime;
        }
        canCast = true;
    }
}
