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

        var notif = ToastSystem.Instance.SendNotification("Select a group of units by holding left click then dragging the mouse.", NotificationPriority.Low, false);

        input.actions["Select"].Enable();

        while (controlSystem.controlledUnits.Count != 4)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2f);
        ToastSystem.Instance.DismissNotification(notif);
        cam_move.enabled = true;

        var notif4 = ToastSystem.Instance.SendNotification("Make your units attack an area by clicking 'Q' then left clicking the ground.", NotificationPriority.Low, false);
        var notif5 = ToastSystem.Instance.SendNotification("Defeat the enemy units to continue.", NotificationPriority.Low, false);

        input.actions.FindActionMap("Player").Enable();
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

    }
}
