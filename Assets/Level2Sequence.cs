using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level2Sequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        
        ToastSystem.instance.SendDialogue("Select a group of units by holding left click then dragging the mouse.", autoDismiss: false);
        input.actions["Select"].Enable();

        while (controlSystem.controlledUnits.Count != 4)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2f);
        ToastSystem.instance.AdvanceDialogue();
        cam_move.enabled = true;

        ToastSystem.instance.SendDialogue("There are two types of movement commands.", autoDismiss: false);
        ToastSystem.instance.SendDialogue("Move command with right click only moves your units.", autoDismiss: false);
        ToastSystem.instance.SendDialogue("When give a move command, units will not attack until they either reach the given location or given an attack move command.", autoDismiss: false);
        ToastSystem.instance.SendDialogue("When give an attack move command, your units will try to move to the location, but if they spot an enemy they will attack.", autoDismiss: false);


        ToastSystem.instance.SendDialogue("Give your units an attack move command by clicking 'Q' then left clicking the ground.", autoDismiss: false);
        
        input.actions["Activate Attack"].Enable();

        while (true)
        {
            //look at control system to get names of a specific actions
            if (input.actions["Activate Attack"].WasPerformedThisFrame())
            {
                break;
            }
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        input.actions.FindActionMap("Player").Enable();
        ToastSystem.instance.AdvanceDialogue();

        ToastSystem.instance.SendDialogue("Defeat the enemy units to continue.", autoDismiss: false);
    }
}
