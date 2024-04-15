using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FinalBattle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(finale());
    }

    IEnumerator finale()
    {
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        yield return new WaitForSeconds(0.1f);
        ToastSystem.instance.SendDialogue("Is that...?", autoDismiss: false);
        ToastSystem.instance.SendDialogue("Quickly! Back into position!", autoDismiss: false);
        ToastSystem.instance.SendDialogue("But sir! What about the Seleneians?", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);
        ToastSystem.instance.SendDialogue("Get into position with them! The incoming Astran force is too large for us to fight alone!", autoDismiss: false);
        ToastSystem.instance.SendDialogue("Move move move!", autoDismiss: false);
        ToastSystem.instance.SendDialogue("Sir yes sir!", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);

        yield return new WaitForSeconds(7f);
        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;
    }
}
