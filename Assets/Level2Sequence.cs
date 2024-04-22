using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level2Sequence : MonoBehaviour
{
    public GameObject enemyUnits;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        
        ToastSystem.instance.SendDialogue("Select a group of units by holding left click then dragging the mouse.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 1-[AudioTrimmer.com]"));
        input.actions["Select"].Enable();

        var selectObj = ToastSystem.instance.SendObjective("Select your units");
        while (controlSystem.controlledUnits.Count != 4)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        ToastSystem.instance.CompleteObjective(selectObj);

        yield return new WaitForSeconds(2f);
        ToastSystem.instance.AdvanceDialogue();
        cam_move.enabled = true;

        ToastSystem.instance.SendDialogue("There are two types of movement commands.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 2-[AudioTrimmer.com]"));
        ToastSystem.instance.SendDialogue("Move command with right click only moves your units.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 3-[AudioTrimmer.com]"));
        ToastSystem.instance.SendDialogue("When given a move command, units will not attack until they either reach the given location or given an attack move command.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 4-[AudioTrimmer.com]"));
        ToastSystem.instance.SendDialogue("When given an attack move command, your units will try to move to the location, but if they spot an enemy they will attack.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 5-[AudioTrimmer.com]"));


        ToastSystem.instance.SendDialogue("Give your units an attack move command by clicking 'Q' then left clicking the ground.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 6-[AudioTrimmer.com]"));
        
        input.actions["Activate Attack"].Enable();
        var attackObj = ToastSystem.instance.SendObjective("Attack Move");
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
        ToastSystem.instance.CompleteObjective(attackObj);
        input.actions.FindActionMap("Player").Enable();
        ToastSystem.instance.AdvanceDialogue();

        ToastSystem.instance.SendDialogue("Defeat the enemy units to continue.", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 2 Line 7-[AudioTrimmer.com]"));

        var defeatEnemies = ToastSystem.instance.SendObjective("Defeat the enemy units");
        while(enemyUnits.transform.childCount >= 0) {
            yield return null;
        }
        ToastSystem.instance.CompleteObjective(defeatEnemies);
    }
}
