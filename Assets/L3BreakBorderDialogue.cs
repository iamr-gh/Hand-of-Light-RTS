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
        portraitLabel: "Commando", portrait: GlobalUnitManager.singleton.GetPortrait("Commando").Item1, autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/Commando Lines/AmirNassiri_CommandoLinesOne"));


        //make general speaker
        ToastSystem.instance.SendDialogue("Alright, commandos, infiltrate the enemy base. There's a small clearing you can teleport units in from. Tear them apart from the inside.",
        //using ranged for general rn
        portraitLabel: "General", autoDismissTime: 10f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_7"));


        ToastSystem.instance.SendDialogue("Roger that.",
        portraitLabel: "Commando", portrait: GlobalUnitManager.singleton.GetPortrait("Commando").Item1, autoDismissTime: 2f, audioClip: Resources.Load<AudioClip>("Audio/Commando Lines/AmirNassiri_CommandoLinesTwo"));

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
        portraitLabel: "Enemy Knight", portrait: GlobalUnitManager.singleton.GetPortrait("EnemyMelee").Item1, autoDismissTime: 5f);

        //yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("Treachery?",
        portraitLabel: "General", autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_8"));

        //yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("General, what's our next move?",
        portraitLabel: "Soldier", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/soldier story voice lines/General_whats_our_next_move_Soldier"));

       //yield return new WaitForSeconds(4f);

        ToastSystem.instance.SendDialogue("For now, we regroup and join up with the rest of our forces.",
        portraitLabel: "General", autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_9"));

        //yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("They'll likely regroup and counterattack and we can't afford to be caught by a larger force.",
        portraitLabel: "General", autoDismissTime: 3f);

        //yield return new WaitForSeconds(3f);

        //level complete, move onto the next one
        ToastSystem.instance.SendDialogue("Copy that.",
        portraitLabel: "Soldier", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/soldier story voice lines/Copy_that_Soldier"));

        yield return new WaitForSeconds(4f);

        // ToastSystem.instance.SendDialogue("Congratulations, you have completed the game!", autoDismissTime: 5.0f);
        // yield return new WaitForSeconds(5f);
        //load next level
        //NO LONGER LAST LEVEL
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}