using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Tutorial4Sequence : MonoBehaviour
{
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
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        ToastSystem.instance.SendDialogue("Some special units have abilities, which will appear in an additional menu", autoDismissTime: 5f);
        yield return new WaitForSeconds(5f);

        var notif1 = ToastSystem.instance.SendNotification("Select the Commando at the top of the screen", autoDismiss: false);
        input.actions["Select"].Enable();
        
        while(true){
            yield return null;
            bool found = false;
            foreach (var unit in controlSystem.controlledUnits)
            {
                if(unit.TryGetComponent(out Ability ability)){
                    found = true;
                    break;
                }
            }
            if (found){
                break;
            }
        }

        ToastSystem.instance.DismissNotification(notif1);
        
        input.actions["Ability 1"].Enable();


        // ToastSystem.instance.SendDialogue("Press 1 and click to use the Commando's ability to recall your units", autoDismiss: false);

        var notif6 = ToastSystem.instance.SendNotification("Press 1 and click your units to use Commando's recall!", false);
        
        //get positions of existing non-commando units
        Dictionary<GameObject, Vector3> unitPositions = new();
        foreach(var unit in GlobalUnitManager.singleton.allManaged){
            if(!unit.TryGetComponent(out Ability ability)){
                unitPositions[unit] = unit.transform.position;
            }
        }

        while (true)
        {
            yield return null;
            bool allRecalled = true;
            foreach (var unit in GlobalUnitManager.singleton.allManaged)
            {
                if (!unit.TryGetComponent(out Ability ability))
                {
                    if (unit.transform.position == unitPositions[unit])
                    {
                        allRecalled = false;
                        break;
                    }
                }
            }
            if (allRecalled)
            {
                break;
            }
        }
        ToastSystem.instance.DismissNotification(notif6);

        ToastSystem.instance.AdvanceDialogue();
        input.actions.FindActionMap("Player").Enable();

        cam_move.enabled = true;

        ToastSystem.instance.SendDialogue("Tutorial Complete!", autoDismiss: false);

        //increment to next scene's build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

