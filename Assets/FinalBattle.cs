using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FinalBattle : MonoBehaviour
{
    //they should all begin attack moving you
    public GameObject allEnemies;
    public Vector3 attackMoveLocation;

    void Start()
    {
        StartCoroutine(finale());
    }

    IEnumerator finale()
    {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        //yield return new WaitForSeconds(0.1f);
        //ToastSystem.instance.SendDialogue("Is that...?", autoDismiss: false);
        //while (!Input.GetKeyDown(KeyCode.Space)) {
        //    yield return null;
        //}
        //ToastSystem.instance.SendDialogue("Quickly! Back into position!", autoDismiss: false);
        //while (!Input.GetKeyDown(KeyCode.Space)) {
        //    yield return null;
        //}
        //ToastSystem.instance.SendDialogue("But sir! What about the Seleneians?", portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);
        //while (!Input.GetKeyDown(KeyCode.Space)) {
        //    yield return null;
        //}
        //ToastSystem.instance.SendDialogue("Get into position with them! The incoming Astran force is too large for us to fight alone!", autoDismiss: false);
        //while (!Input.GetKeyDown(KeyCode.Space)) {
        //    yield return null;
        //}
        //ToastSystem.instance.SendDialogue("Move move move!", autoDismiss: false);
        //while (!Input.GetKeyDown(KeyCode.Space)) {
        //    yield return null;
        //}
        //ToastSystem.instance.SendDialogue("Sir yes sir!", portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);

        ToastSystem.instance.SendDialogue("That's a lot of enemy troops.",
        portraitLabel: "General", autoDismissTime: 10f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_18"));

        ToastSystem.instance.SendDialogue("Alright! Let's utilize every tactic we've employed to make it this far! Let's crush them and go home!",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Yes sir!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine15"));
        

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        //give them some time to look at what they have
        yield return new WaitForSeconds(15f);
        
        //move all enemies to the player
        for(int i=0;i<allEnemies.transform.childCount;i++)
        {
            
            if(allEnemies.transform.GetChild(i).TryGetComponent(out UnitAI ai))
            {
                ai.AttackMoveToCoordinate(attackMoveLocation);
            }
        }
    }
}
