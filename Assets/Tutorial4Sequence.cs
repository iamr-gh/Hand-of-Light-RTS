using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Tutorial4Sequence : MonoBehaviour {
    public GameObject enemyToKill;
    
    public GameObject magesToUse;
    // Start is called before the first frame update
    
    bool dialogueDismissed = false;
    
    void Start() {
        ToastSystem.instance.onDialogueAdvanced.AddListener(dismissTracker);
        StartCoroutine(tutorial());
    }
    
    void dismissTracker() {
        //could be switched to a counter at some point, but I think this is easier
        dialogueDismissed = true;
    }
    
    

    IEnumerator tutorial() {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("Some special units have abilities, which will appear in an additional menu", autoDismissTime: 5f);
        while(!dialogueDismissed) {
            yield return null;
        }

        var notif1 = ToastSystem.instance.SendNotification("Select the Commando at the top of the screen", autoDismiss: false);
        input.actions["Select"].Enable();

        while (true) {
            yield return null;
            bool found = false;
            foreach (var unit in controlSystem.controlledUnits) {
                if (unit.TryGetComponent(out Ability ability)) {
                    found = true;
                    break;
                }
            }
            if (found) {
                break;
            }
        }

        ToastSystem.instance.DismissNotification(notif1);

        input.actions["Ability 1"].Enable();


        // ToastSystem.instance.SendDialogue("Press 1 and click to use the Commando's ability to recall your units", autoDismiss: false);

        var notif6 = ToastSystem.instance.SendNotification("Press 1 and click your units to use Commando's recall!", false);

        //get positions of existing non-commando units
        Dictionary<GameObject, Vector3> unitPositions = new();
        foreach (var unit in GlobalUnitManager.singleton.allManaged) {
            if (!unit.TryGetComponent(out Ability ability)) {
                //hard coded
                if (unit.TryGetComponent(out UnitAffiliation affiliation) && affiliation.affiliation == "White") {
                    unitPositions[unit] = unit.transform.position;
                }
            }
        }

        while (true) {
            yield return null;
            bool allRecalled = true;
            foreach (var unit in GlobalUnitManager.singleton.allManaged) {
                if (!unit.TryGetComponent(out Ability ability)) {
                    if (unit.TryGetComponent(out UnitAffiliation affiliation) && affiliation.affiliation == "White") {
                        if (unit.transform.position == unitPositions[unit]) {
                            allRecalled = false;
                            break;
                        }
                    }
                }
            }
            if (allRecalled) {
                break;
            }
        }
        ToastSystem.instance.DismissNotification(notif6);

        ToastSystem.instance.AdvanceDialogue();

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("Recall can be used to get your units out of sticky situations.", autoDismissTime: 3.0f);
        while(!dialogueDismissed) {
            yield return null;
        }

        //kill enemy unit
        ToastSystem.instance.SendDialogue("It also works on enemy units, use it to engage the one on the plateau", autoDismiss: false);


        while(true) {
            yield return null;
            //wait till death
            if (enemyToKill == null) {
                break;
            }
        }
        
        ToastSystem.instance.AdvanceDialogue();

        //wait till death of this unit
        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("As seen, this is a double edged sword.", autoDismissTime: 3.0f);

        while(!dialogueDismissed) {
            yield return null;
        }

        //honestly just make another tutorial

        //maybe some camera pan
        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("There are other units with abilities", autoDismissTime: 3.0f);
        
        while(!dialogueDismissed) {
            yield return null;
        }
        

        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("These mages can blind archers, to stop them from attacking within a zone", autoDismiss: false);
        magesToUse.SetActive(true);
        
        while(!dialogueDismissed) {
            yield return null;
        }
        
        //now force mages to do something
        //come up with a way to introduce mage
        
        




        //how I transition to mages?       
        // will be another tutorial??

        // should this have a portrait?
        ToastSystem.instance.SendDialogue("Promotion Exam Complete! Let's get you into the field.", autoDismiss: false);

        yield return new WaitForSeconds(5f);

        //increment to next scene's build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

