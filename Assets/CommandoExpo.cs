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
        Vector3 newCameraPosition = finalCameraPosition;
        yield return StartCoroutine(ScrollCamera(cameraStartPosition, newCameraPosition, 3f));
        StartCoroutine(NotifyPlayerOfCommando());
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(ScrollCamera(Camera.main.transform.position, cameraStartPosition, 3f));

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

    IEnumerator NotifyPlayerOfCommando() {
        var commandoNotif = ToastSystem.instance.SendNotification("Commando arrived!", false);
        yield return new WaitForSeconds(5f);
        ToastSystem.instance.DismissNotification(commandoNotif);
    }
}
