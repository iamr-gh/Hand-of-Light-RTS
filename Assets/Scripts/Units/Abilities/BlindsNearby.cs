using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlindsNearby : MonoBehaviour {
    //just on collision currently
    // public float effect_radius = 5.0f;

    private Dictionary<GameObject, float> blinded = new();


    //blinds all irrespective of affilieation
    // private string affiliation;
    void OnTriggerEnter(Collider other) {
        GameObject unit = other.gameObject;
        //note this is spherical
        if (unit.TryGetComponent(out RangedWeapon wep)) {
            if (unit.TryGetComponent(out UnitParameters param)) {
                if (!blinded.ContainsKey(unit)) {
                    blinded.Add(unit, param.getAttackRange());
                    param.setAttackRange(0);
                }
            }
            //what if we just set range to 0
            // wep.enabled = false;
            // }
            // // var around = GlobalUnitManager.singleton.FindNearby(transform.position, effect_radius);                
            // foreach(GameObject unit in around){
            // }
        }
    }

    void OnTriggerExit(Collider other) {
        GameObject unit = other.gameObject;
        // var around = GlobalUnitManager.singleton.FindNearby(transform.position, effect_radius);                
        // foreach(GameObject unit in around){
        if (blinded.ContainsKey(unit)) {
            if (unit.TryGetComponent(out UnitParameters param)) {
                param.setAttackRange(blinded[unit]);
                blinded.Remove(unit);
            }
        }
        // }
    }

    void OnDestroy() {

        //unblind all blinded units
        foreach (GameObject unit in blinded.Keys) {
            if(unit != null){
                if (unit.TryGetComponent(out UnitParameters param)) {
                    param.setAttackRange(blinded[unit]);
                }
            }
        }
    }
}
