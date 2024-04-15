using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoExpo : MonoBehaviour
{
    public GameObject commando;
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
        Vector3 cameraStartPosition = Camera.main.transform.position;
        Vector3 newCameraPosition = new Vector3(commando.transform.position.x, cameraStartPosition.y, commando.transform.position.z - 20f);
        //yield return StartCoroutine(ScrollCamera(cameraStartPosition, newCameraPosition, 1f));
        yield return new WaitForSeconds(0.5f);
        //yield return StartCoroutine(ScrollCamera(Camera.main.transform.position, cameraStartPosition, 1f));
    }

    IEnumerator ScrollCamera(Vector3 initialPosition, Vector3 finalPosition, float duration_sec) {
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / duration_sec;
        while (progress < 1.0f) {
            progress = (Time.time - initial_time) / duration_sec;
            Vector3 new_position = Vector3.Lerp(initialPosition, finalPosition, progress);
            Camera.main.transform.position = new_position;
            yield return null;
        }
        Camera.main.transform.position = finalPosition;
    }
}
