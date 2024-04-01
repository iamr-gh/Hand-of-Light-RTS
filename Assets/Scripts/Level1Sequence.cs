using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

public class Level1Sequence : MonoBehaviour
{
    bool step1Camera = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();


        //EX. disable all controls
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        ToastSystem.Instance.SendNotification("Controls disabled",NotificationPriority.Low,true,2f);
        yield return new WaitForSeconds(3f);
        //reenable controls
        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;
        ToastSystem.Instance.SendNotification("Controls enabled",NotificationPriority.Low,true,2f);

        yield return new WaitForSeconds(3f);

        
        ToastSystem.Instance.SendNotification("Here is an enemy encampment, eliminate it",NotificationPriority.Low,true,2f);
        
        // TODO: Move Camera, using 494 coroutine lib now included        
        
        //autodismiss set to false
        var notif = ToastSystem.Instance.SendNotification("Move your camera by moving mouse to edges of screen",NotificationPriority.Low,false);

        var cam_pos_before = Camera.main.transform.position;

        input.actions.FindActionMap("Player").Disable();
        //wait until user moved camera, yield return null is very important
        while(cam_pos_before == Camera.main.transform.position){
           yield return null; 
        }
        
        // ToastSystem.Instance.AdvanceDialogue();
        ToastSystem.Instance.DismissNotification(notif);
        
        yield return new WaitForSeconds(3f);

        //example of after moving camera things
        ToastSystem.Instance.SendNotification("Good Job moving camera",NotificationPriority.Low,true,2f);

        yield return new WaitForSeconds(2f);

        ToastSystem.Instance.SendNotification("Move your camera over your units",NotificationPriority.Low,true,2f);
        
        //if you want to force this, figure out some criteria with camera position
        yield return new WaitForSeconds(2f);

        
        //for more fine grained enabling, do input.actions["action name"].Disable()
        input.actions.FindActionMap("Player").Enable();

        notif = ToastSystem.Instance.SendNotification("Click on a unit to select it",NotificationPriority.Low,false);

        // GlobalUnitManager.singleton.TryGetComponent(out ControlSystem controlSystem);
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //If you just want to be input based, input.actions["action name"].WasPressedThisFrame() will tell you
        //however a lot of this is about qualities
        while(controlSystem.controlledUnits.Count == 0 ) {
            yield return null;
        }
        
        ToastSystem.Instance.DismissNotification(notif);
        
        notif = ToastSystem.Instance.SendNotification("Right click on the map to tell that unit to move",NotificationPriority.Low,false);
        
        while(true){
            //look at control system to get names of a specific actions
            if(input.actions["Move"].WasPerformedThisFrame()){
                break;
            }
            yield return null;
        }

        ToastSystem.Instance.DismissNotification(notif);
        //now you can do more properties related to unit selection
        
       //do left click and drag to select units in a box, use controlledUnits.Count to verify it's what you want 
       //input.actions["Attack"] for attack move tracking
    }
}
