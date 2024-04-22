using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class L2KingHillDialogue : MonoBehaviour
{
    public GameObject enemy;
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


        ToastSystem.instance.SendDialogue("General. We've located an enemy encampment.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 6f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine6"));

        ToastSystem.instance.SendDialogue("We believe this is where their forces have been moving through to avoid detection.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 6f);

        //make general speaker
        ToastSystem.instance.SendDialogue("It's a good tactical location, but they've chosen a poor position for a fight.",
        //using ranged for general rn
        portraitLabel: "General", autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_4"));


        //make general speaker
        ToastSystem.instance.SendDialogue("Archers, sieze the high ground and snipe them from afar.",
        //using ranged for general rn
        portraitLabel: "General", autoDismissTime: 5f);



        ToastSystem.instance.SendDialogue("Yes sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 2f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine7"));

        ToastSystem.instance.SendDialogue("Knights, forward!",
        portraitLabel: "General", autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_5"));


        yield return new WaitForSeconds(5f);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        while (enemy.transform.GetChild(0).childCount + enemy.transform.GetChild(1).childCount + enemy.transform.GetChild(2).childCount + enemy.transform.GetChild(3).childCount != 0)
        {
            yield return null;
        }

        //on win

        ToastSystem.instance.SendDialogue("Enemy base has been cleared, sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine8"));

        yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("We need to move fast. The unit here has likely already contacted the nearest base. \r\n",
        portraitLabel: "General", autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_6"));

        yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("We need to blitz through enemy territory and take that base out before the enemy can regroup and launch a counterattack.",
        portraitLabel: "General", autoDismissTime: 7f);

        ToastSystem.instance.SendDialogue("Call for a commando and speeder unit.",
        portraitLabel: "General", autoDismissTime: 7f);

        yield return new WaitForSeconds(7f);

        //level complete, move onto the next one
        ToastSystem.instance.SendDialogue("Understood.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 2f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine9"));

        yield return new WaitForSeconds(2f);


        //load next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}