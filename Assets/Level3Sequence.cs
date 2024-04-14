using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level3Sequence : MonoBehaviour
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

        ToastSystem.instance.SendDialogue("Select a single unit type by holding control then clicking on a unit, or by double clicking. Try selecting the archers.", autoDismiss: false);
        input.actions["Select"].Enable();
        while (controlSystem.controlledUnits.Count != 5)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        

        yield return new WaitForSeconds(2f);
        ToastSystem.instance.AdvanceDialogue();

        ToastSystem.instance.SendDialogue("There is a technique known as kiting. As you fight this next battle, try control clicking your archers. Then make them retreat by moving them, then pressing attack move, and repeating.", autoDismiss: false);

        var notif6 = ToastSystem.instance.SendNotification("Press space to continue.", false);

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        ToastSystem.instance.DismissNotification(notif6);

        ToastSystem.instance.AdvanceDialogue();
        input.actions.FindActionMap("Player").Enable();

        cam_move.enabled = true;

        ToastSystem.instance.SendDialogue("Defeat the enemies at the top of the map to continue.", autoDismiss: false);
    }
}

/*
 * var notif = ToastSystem.Instance.SendNotification("Select a single unit type by holding control then left click.", NotificationPriority.Low, false);
        var notif2 = ToastSystem.Instance.SendNotification("Select a single unit type by double clicking.", NotificationPriority.Low, false);
        var notif3 = ToastSystem.Instance.SendNotification("Select the archers.", NotificationPriority.Low, false);

        input.actions.FindActionMap("Player").Enable();

        while (controlSystem.controlledUnits.Count != 8)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2f);
        ToastSystem.Instance.DismissNotification(notif);
        ToastSystem.Instance.DismissNotification(notif2);
        ToastSystem.Instance.DismissNotification(notif3);
        cam_move.enabled = true;

        var notif4 = ToastSystem.Instance.SendNotification("If you press 'S' as your units are moving, they will stop.", NotificationPriority.Low, false);

        while (true)
        {
            //look at control system to get names of a specific actions
            if (input.actions["Stop"].WasPerformedThisFrame())
            {
                break;
            }
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2f);
        ToastSystem.Instance.DismissNotification(notif4);

        ToastSystem.Instance.SendNotification("There is a technique known as kiting.", NotificationPriority.Low, true, 5f);
        yield return new WaitForSeconds(5f);
        ToastSystem.Instance.SendNotification("As you fight this next battle, try control clicking your archers.", NotificationPriority.Low, true, 5f);
        ToastSystem.Instance.SendNotification("Then make them retreat by moving them, then pressing stop, and repeating.", NotificationPriority.Low,
 */