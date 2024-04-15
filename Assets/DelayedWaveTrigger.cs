using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedWaveTrigger : MonoBehaviour
{
    public GameObject units;
    [SerializeField] float wait_seconds = 15f;
    private L1AttackWave wave;
    private bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        wave = units.GetComponent<L1AttackWave>();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggering: " + other.gameObject.name);
        UnitAffiliation affil = other.gameObject.GetComponent<UnitAffiliation>();
        if (!triggered && affil != null && affil.affiliation == "White") {
            StartCoroutine(StartWave(wait_seconds));
        }
    }

    IEnumerator StartWave(float wait_seconds) {
        yield return new WaitForSeconds(wait_seconds);
        wave.start = true;
    }
}
