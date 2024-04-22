using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoExpo : MonoBehaviour
{
    public GameObject commando;
    [SerializeField] Vector3 finalCameraPosition;
    bool triggered = false;

    private void OnTriggerEnter(Collider other) {
        UnitAffiliation affiliation = other.gameObject.GetComponent<UnitAffiliation>();
        if (!triggered && affiliation != null && affiliation.affiliation == "White") {
            StartCoroutine(SeeCommando());
            triggered = true;
        }
    }

    IEnumerator SeeCommando() {
        commando.SetActive(true);
        GlobalUnitManager.singleton.Reindex();
        Time.timeScale = 0f; // Pause Game

        Vector3 cameraStartPosition = Camera.main.transform.position;
        yield return StartCoroutine(ScrollCamera(cameraStartPosition, finalCameraPosition, 2f));
        StartCoroutine(NotifyPlayerOfCommando());
        yield return StartCoroutine(WaitForRealSeconds(2f));
        yield return StartCoroutine(ScrollCamera(finalCameraPosition, cameraStartPosition, 2f));

        Time.timeScale = 1f; // Unpause Game
    }

    IEnumerator ScrollCamera(Vector3 initialPosition, Vector3 finalPosition, float duration_sec) {
        float initial_time = Time.realtimeSinceStartup;
        float progress = (Time.realtimeSinceStartup - initial_time) / duration_sec;
        while (progress < 1.0f) {
            progress = (Time.realtimeSinceStartup - initial_time) / duration_sec;
            Vector3 new_position = Vector3.Lerp(initialPosition, finalPosition, progress);
            Camera.main.transform.position = new_position;
            yield return null;
        }
        Camera.main.transform.position = finalPosition;
    }

    IEnumerator WaitForRealSeconds(float duration_sec) {
        float initialTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - initialTime < duration_sec) { yield return null; }
    }

    IEnumerator NotifyPlayerOfCommando() {
        var commandoNotif = ToastSystem.instance.SendNotification("Commando arrived!", false);
        yield return StartCoroutine(WaitForRealSeconds(5f));
        ToastSystem.instance.DismissNotification(commandoNotif);
    }

    
}
