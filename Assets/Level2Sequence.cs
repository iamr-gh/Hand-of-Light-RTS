using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

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

        ToastSystem.Instance.SendNotification("Now that you can lead your army.", NotificationPriority.Low, true, 3f);
        ToastSystem.Instance.SendNotification("You've been reinforced by several archers.", NotificationPriority.Low, true, 3f);
        yield return new WaitForSeconds(3f);

        var notif = ToastSystem.Instance.SendNotification("Select a single unit type by holding control then left click.", NotificationPriority.Low, false);
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
    }

}
