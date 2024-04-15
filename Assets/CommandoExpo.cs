using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandoExpo : MonoBehaviour
{
    public GameObject commando;

    private void OnTriggerEnter(Collider other) {
        UnitAffiliation affiliation = other.gameObject.GetComponent<UnitAffiliation>();
        if (affiliation != null && affiliation.affiliation == "White") {
            StartCoroutine(SeeCommando());
        }
    }

    IEnumerator SeeCommando() {
        commando.SetActive(true);
        Vector3 cameraStartPosition = Camera.main.transform.position;
        yield return StartCoroutine(ScrollCamera(cameraStartPosition, commando.transform.position, 0.5f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(ScrollCamera(Camera.main.transform.position, cameraStartPosition, 0.5f));
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
