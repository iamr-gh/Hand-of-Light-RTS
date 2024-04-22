using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

public class Level1Sequence : MonoBehaviour
{
    public nextlevel nl;
    //bool step1Camera = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();


        //EX. disable all controls
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        yield return new WaitForSeconds(1f);
        ToastSystem.instance.SendDialogue("Commander! You're running late for your promotion exam! Press space to advance dialogue.", autoDismiss: false, audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/tutorial-1-line-1_t178hwGV"));
        ToastSystem.instance.SendDialogue("Use WASD or move your mouse to the edges of the screen to move the camera.", autoDismiss: false);
        var obj1 = ToastSystem.instance.SendObjective("Move the camera");

        cam_move.enabled = true;
        input.actions["Pan Camera"].Enable();
        var cam_pos_before = Camera.main.transform.position;
        while (cam_pos_before == Camera.main.transform.position)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        ToastSystem.instance.CompleteObjective(obj1);
        ToastSystem.instance.AdvanceDialogue();

        ToastSystem.instance.SendDialogue("Now select a troop with left click. You need to select units to issue orders.", autoDismiss: false);

        var obj2 = ToastSystem.instance.SendObjective("Select a troop");

        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();
        input.actions["Select"].Enable();
        while (controlSystem.controlledUnits.Count == 0)
        {
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        ToastSystem.instance.CompleteObjective(obj2);
        ToastSystem.instance.AdvanceDialogue();
        ToastSystem.instance.SendDialogue("Right click to issue a move command order. The unit will move to the location you right click.", autoDismiss: false);
        input.actions["Select"].Disable();
        input.actions["Move"].Enable();
        
        var obj3 = ToastSystem.instance.SendObjective("Move your troop");

        while (true)
        {
            //look at control system to get names of a specific actions
            if (input.actions["Move"].WasPerformedThisFrame())
            {
                break;
            }
            yield return null;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        ToastSystem.instance.CompleteObjective(obj3);
        //yield return new WaitForSeconds(2f);
        ToastSystem.instance.AdvanceDialogue();

        input.actions["Select"].Enable();

        ToastSystem.instance.SendDialogue("Move through the pass to the east to continue.", autoDismiss: false);

        finishObj = ToastSystem.instance.SendObjective("Move your troop into the green box");

        nl.onAdvancing.AddListener(Finish);
        /*
        //reenable controls
        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;
        ToastSystem.Instance.SendNotification("Controls enabled",NotificationPriority.Low,true,2f);

        yield return new WaitForSeconds(3f);

        
        ToastSystem.Instance.SendNotification("Here is an enemy encampment, eliminate it",NotificationPriority.Low,true,2f);
        
        // TODO: Move Camera, using 494 coroutine lib now included        
        
        //autodismiss set to false
        var notif = ToastSystem.Instance.SendNotification("Move your camera by moving mouse to edges of screen",NotificationPriority.Low,false);

       

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
        */
    }

    ulong finishObj;
    void Finish() {
        ToastSystem.instance.CompleteObjective(finishObj);
        nl.onAdvancing.RemoveListener(Finish);
    }
}
