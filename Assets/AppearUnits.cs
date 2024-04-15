using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearUnits : MonoBehaviour
{
    public GameObject[] units;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other) {
        UnitAffiliation affiliation = other.gameObject.GetComponent<UnitAffiliation>();
        if (!alreadyTriggered && affiliation != null && affiliation.affiliation == "White") {
            foreach (GameObject unit in units) {
                unit.SetActive(true);
            }
            alreadyTriggered = true;
        }
    }
}
