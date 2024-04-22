using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Tutorial4Sequence : MonoBehaviour {
    public GameObject enemyToKill;
    
    public GameObject magesToUse;

    // let's just dactivate commando when we're done with it 
    public GameObject commandoToUse;
    
    public GameObject archersToUse;
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
        ToastSystem.instance.SendDialogue("Some special units have abilities, which will appear in an additional menu", autoDismissTime: 7f
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 1-[AudioTrimmer.com]"));
        while(!dialogueDismissed) {
            yield return null;
        }

        var notif1 = ToastSystem.instance.SendNotification("Select the Commando at the top of the screen", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 2-[AudioTrimmer.com]"));
        input.actions["Select"].Enable();
        
        var select_commando_obj = ToastSystem.instance.SendObjective("Select the Commando");

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
        ToastSystem.instance.CompleteObjective(select_commando_obj);

        ToastSystem.instance.DismissNotification(notif1);

        input.actions["Ability 1"].Enable();


        // ToastSystem.instance.SendDialogue("Press 1 and click to use the Commando's ability to recall your units", autoDismiss: false);

        var notif6 = ToastSystem.instance.SendNotification("Press 1 and click your units to use Commando's recall!", false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 3-[AudioTrimmer.com]"));

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
        
        ToastSystem.instance.RemoveObjective(select_commando_obj);
        var recall_friend_obj = ToastSystem.instance.SendObjective("Recall your units over the ridge");

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
        ToastSystem.instance.CompleteObjective(recall_friend_obj);
        ToastSystem.instance.DismissNotification(notif6);

        ToastSystem.instance.AdvanceDialogue();

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("Recall can be used to get your units out of sticky situations.", autoDismissTime: 5f
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 4-[AudioTrimmer.com]"));
        while(!dialogueDismissed) {
            yield return null;
        }

        //kill enemy unit
        ToastSystem.instance.SendDialogue("It also works on enemy units, use it to engage the one on the plateau", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 5-[AudioTrimmer.com]"));
        
        var recall_enemy_obj = ToastSystem.instance.SendObjective("Kill enemy on plateau");


        while(true) {
            yield return null;
            //wait till death
            if (enemyToKill == null) {
                break;
            }
        }
        
        ToastSystem.instance.CompleteObjective(recall_enemy_obj);
        
        ToastSystem.instance.AdvanceDialogue();

        //wait till death of this unit
        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("As seen, this is a double edged sword.", autoDismissTime: 5f
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 6-[AudioTrimmer.com]"));

        while(!dialogueDismissed) {
            yield return null;
        }

        //honestly just make another tutorial

        //maybe some camera pan
        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("There are other units with abilities", autoDismissTime: 3.0f
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 7-[AudioTrimmer.com]"));

        
        while(!dialogueDismissed) {
            yield return null;
        }
        

        dialogueDismissed = false;
        ToastSystem.instance.SendDialogue("These mages can blind archers, to stop them from attacking within a zone", autoDismissTime: 3.0f
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 8-[AudioTrimmer.com]"));
        magesToUse.SetActive(true);
        commandoToUse.SetActive(false);
        
        GlobalUnitManager.singleton.Reindex();
        
        while(!dialogueDismissed) {
            yield return null;
        }
        
        
        dialogueDismissed = false;
        var blindarchnot = ToastSystem.instance.SendNotification("Blind the archers on the plateau to protect your units", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 9-[AudioTrimmer.com]"));
        archersToUse.SetActive(true);
        GlobalUnitManager.singleton.Reindex();
        
        var blind_archers_obj = ToastSystem.instance.SendObjective("Blind archers on plateau");
        while(true){
            //check if archers ever blinded
            //check if attack range is zero in archers to use children
            bool foundBlind = false;
            for(int i=0; i < archersToUse.transform.childCount;i++){
                if(archersToUse.transform.GetChild(i).TryGetComponent(out UnitParameters param)){
                    if(param.getAttackRange() == 0){
                        foundBlind = true;
                        break;
                    }
                }
            }
            
            if(foundBlind){
                break;
            }
            yield return null;
        }
        ToastSystem.instance.CompleteObjective(blind_archers_obj);
        ToastSystem.instance.AdvanceDialogue();
        
        //now force mages to do something
        //come up with a way to introduce mage
        
        //introduce archers on platform? 

        //how do I verify use?


        //how I transition to mages?       
        // will be another tutorial??

        // should this have a portrait?
        ToastSystem.instance.DismissNotification(blindarchnot);
        ToastSystem.instance.SendDialogue("Congratulations, you are now qualified to use spellcasters in the field!", autoDismiss: false
            , audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Tutorial 4 Line 10-[AudioTrimmer.com]"));

        yield return new WaitForSeconds(7f);

        //increment to next scene's build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

