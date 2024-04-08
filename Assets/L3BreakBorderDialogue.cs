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
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;


        ToastSystem.Instance.SendDialogue("Commando and speeder unit, reporting for duty.",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 4f);

        yield return new WaitForSeconds(4f);

        //make general speaker
        ToastSystem.Instance.SendDialogue("Alright, commandos, infiltrate the enemy base. There’s a small clearing you can teleport units in from. Tear them apart from the inside.",
        //using ranged for general rn
        portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 10f);

        yield return new WaitForSeconds(10f);

        ToastSystem.Instance.SendDialogue("Roger that.",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 2f);

        yield return new WaitForSeconds(2f);

        var notif6 = ToastSystem.Instance.SendNotification("Press 1 to use a units ability.", NotificationPriority.Low, false);
        yield return new WaitForSeconds(4f);
        ToastSystem.Instance.DismissNotification(notif6);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        while (!(enemy.transform.childCount == 1))
        {
            yield return null;
        }

        while (!(enemy2.transform.childCount == 0))
        {
            yield return null;
        }

        //on win

        ToastSystem.Instance.SendDialogue("Stellen scum! You’ll pay for this treachery!",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        yield return new WaitForSeconds(5f);

        ToastSystem.Instance.SendDialogue("Treachery…?",
        portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.Instance.SendDialogue("General, what’s our next move?",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 4f);

        yield return new WaitForSeconds(4f);

        ToastSystem.Instance.SendDialogue("For now, we retreat. They’ll likely regroup and counterattack and we can’t afford to be caught by a larger force.",
        portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 6f);

        yield return new WaitForSeconds(6f);

        //level complete, move onto the next one
        ToastSystem.Instance.SendDialogue("Understood.",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 2f);

        yield return new WaitForSeconds(2f);

        ToastSystem.Instance.SendDialogue("Congratulations, you have completed the game!", autoDismissTime: 5.0f);
        yield return new WaitForSeconds(5f);
        //load next level
        SceneManager.LoadScene(0);

    }
}