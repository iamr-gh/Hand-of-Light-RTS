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
    public AbilityTypes type;
    public int abilitySlot;
    public float aoeRadius;

    public virtual void OnCast(AbilityCastData castData) {

    }
}
