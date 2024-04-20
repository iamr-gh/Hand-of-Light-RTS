using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class L3BreakBorderDialogue : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemy2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(dialogue());
    }

    IEnumerator dialogue()
    {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;


        ToastSystem.instance.SendDialogue("Commando and speeder unit, reporting for duty.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 4f);


        //make general speaker
        ToastSystem.instance.SendDialogue("Alright, commandos, infiltrate the enemy base. There's a small clearing you can teleport units in from. Tear them apart from the inside.",
        //using ranged for general rn
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 10f);


        ToastSystem.instance.SendDialogue("Roger that.",
        portraitLabel: "Commando", portrait: GlobalUnitManager.singleton.GetPortrait("Commando").Item1, autoDismissTime: 2f);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        var notif6 = ToastSystem.instance.SendNotification("Press 1 to use a units ability.", false);
        yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(10f);
        yield return new WaitForSeconds(4f);
        ToastSystem.instance.DismissNotification(notif6);


        while (!(enemy.transform.childCount == 1))
        {
            yield return null;
        }

        while (!(enemy2.transform.childCount == 0))
        {
            yield return null;
        }

        //on win

        ToastSystem.instance.SendDialogue("Stellen scum! You'll pay for this treachery!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("Treachery?",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("General, what's our next move?",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 4f);

        yield return new WaitForSeconds(4f);

        ToastSystem.instance.SendDialogue("For now, we retreat.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("They'll likely regroup and counterattack and we can't afford to be caught by a larger force.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        //level complete, move onto the next one
        ToastSystem.instance.SendDialogue("Understood.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 2f);

        yield return new WaitForSeconds(2f);

        // ToastSystem.instance.SendDialogue("Congratulations, you have completed the game!", autoDismissTime: 5.0f);
        // yield return new WaitForSeconds(5f);
        //load next level
        //NO LONGER LAST LEVEL
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}