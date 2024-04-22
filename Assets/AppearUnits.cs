using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearUnits : MonoBehaviour
{
    public string notificationText;
    public GameObject[] units;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter(Collider other) {
        UnitAffiliation affiliation = other.gameObject.GetComponent<UnitAffiliation>();
        if (!alreadyTriggered && affiliation != null && affiliation.affiliation == "White") {
            foreach (GameObject unit in units) {
                unit.SetActive(true);
            }
            GlobalUnitManager.singleton.Reindex();
            StartCoroutine(NotifyPlayer());
            alreadyTriggered = true;
        }
    }

    IEnumerator NotifyPlayer() {
        var notif = ToastSystem.instance.SendNotification(notificationText, false);
        yield return new WaitForSeconds(5f);
        ToastSystem.instance.DismissNotification(notif);
    }
}
