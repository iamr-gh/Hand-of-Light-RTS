using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class L5_High_Ground_Woes_Dialogue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(dialogue()); 
    }
    
    IEnumerator dialogue(){
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        ToastSystem.instance.onDialogueAdvanced.AddListener(TickDialogue);
        
        
        ToastSystem.instance.SendDialogue("We have forces on the other side of this pass that need reinforcements!",autoDismissTime: 5f);
        // yield return new WaitForSeconds(5f);
        // ToastSystem.instance.SendDialogue("Cross through this pass ASAP!",autoDismissTime: 3f);
        // yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("General, there are archers on the cliffs...",
        portraitLabel: "Melee", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);
        // yield return new WaitForSeconds(5f);
        
        ToastSystem.instance.SendDialogue("Let's use these new mages, they should be able to blind them long enough for you to run past.",autoDismissTime: 5f);
        // yield return new WaitForSeconds(5f);
        
        ToastSystem.instance.SendDialogue("Reporting in, ready to blind some archers.",
        portraitLabel: "Mage", portrait: GlobalUnitManager.singleton.GetPortrait("Mage").Item1, autoDismissTime: 5f); yield return new WaitForSeconds(5f);

        while (dialogueCounter < 4) {
            yield return null;
        }

        ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        var notif6 = ToastSystem.instance.SendNotification("Press 1 to use the Mage's ability to blind archers", false);
        yield return new WaitForSeconds(4f);
        ToastSystem.instance.DismissNotification(notif6);
    }

    int dialogueCounter = 0;
    void TickDialogue() {
        dialogueCounter++;
    }

}
