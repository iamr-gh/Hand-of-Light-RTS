using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

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

        ToastSystem.Instance.SendNotification("The enemy army have found themselves in a good position.", NotificationPriority.Low, true, 3f);
        ToastSystem.Instance.SendNotification("It's unlikely we'll win if we attack head on.", NotificationPriority.Low, true, 3f);
        yield return new WaitForSeconds(3f);

        input.actions.FindActionMap("Player").Enable();

        var notif = ToastSystem.Instance.SendNotification("To help you, cavalry units have reinforced your army.", NotificationPriority.Low, false);
        var notif2 = ToastSystem.Instance.SendNotification("Select all your cavalry units.", NotificationPriority.Low, false);

        while (controlSystem.controlledUnits.Count != 11)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2f);
        ToastSystem.Instance.DismissNotification(notif);
        ToastSystem.Instance.DismissNotification(notif2);

        cam_move.enabled = true;

        ToastSystem.Instance.SendNotification("Split your troops up and attack from multiple angles.", NotificationPriority.Low, true, 5f);
        yield return new WaitForSeconds(2f);
        ToastSystem.Instance.SendNotification("Good luck.", NotificationPriority.Low, true, 5f);
    }
}
