using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;
public class L5EndDialogue : MonoBehaviour {
    private bool started = false;
    void OnTriggerEnter(Collider other) {
        // check if unit is friendly
        if (!started && other.TryGetComponent(out UnitAffiliation unitAffiliation) && unitAffiliation.affiliation == "White") {
            started = true;
            StartCoroutine(dialogue());
        }
    }

    void Start() {
        StartCoroutine(SendObjective());
    }

    ulong objective;
    IEnumerator SendObjective() {
        yield return null;
        yield return null;
        objective = ToastSystem.instance.SendObjective("Make it to the end of the pass");
    }

    IEnumerator dialogue() {
        ToastSystem.instance.CompleteObjective(objective);
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;


        ToastSystem.instance.SendDialogue("We are through the pass, General.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);
        // yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("Excellent work, let's get those reinforcements where they need to be. We can come back with commandos.", autoDismissTime: 5f);
        yield return new WaitForSeconds(8f);
        // yield return null;

        //advance level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
