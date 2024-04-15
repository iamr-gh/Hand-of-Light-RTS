using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class L1HoldPassDialogue : MonoBehaviour
{
    public GameObject firstWave;
    public GameObject secondWave;
    public GameObject thirdWave;

    public GameObject reinforcements;
    public float reinforcementDelay = 40f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(dialogue());
    }

    IEnumerator dialogue()
    {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        ToastSystem.instance.onDialogueAdvanced.AddListener(TickDialogue);

        ToastSystem.instance.SendDialogue("General! We've spotted movement, an advanced Seleneian force is marching on a nearby village.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        //make general speaker
        ToastSystem.instance.SendDialogue("What are we looking at exactly?",
        //using ranged for general rn
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("A few contingents of knights. No other support. Though, some of their advanced party have captured several villagers.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 7f);

        ToastSystem.instance.SendDialogue("Those bastards...",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Right. Our first priority is to hold the mountain passes until reinforcements arrive.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 7f);

        ToastSystem.instance.SendDialogue("We rescue the villagers when we get the opportunity. Move out!",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 7f);

        while (dialogueCounter < 6) {
            yield return null;
        }

        ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        //have a reinforcment condition on timer
        StartCoroutine(triggerReinforcements());

        //start triggering enemy waves
        var awave1 = firstWave.GetComponent<L1AttackWave>();
        awave1.start = true;

        yield return new WaitForSeconds(15f);

        var awave2 = secondWave.GetComponent<L1AttackWave>();
        awave2.start = true;


        yield return new WaitForSeconds(20f);
        var awave3 = thirdWave.GetComponent<L1AttackWave>();
        awave3.start = true;
        
        //wait until all are defeated
        while (!awave1.defeated)
        {
            yield return null;
        }
        while (!awave2.defeated)
        {
            yield return null;
        }
        while (!awave3.defeated)
        {
            yield return null;
        }


        //on win

        ToastSystem.instance.SendDialogue("Enemy forces have been routed sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        yield return new WaitForSeconds(5f);

        ToastSystem.instance.SendDialogue("Good. Now, we need to find out where this force came from.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("We've received no reports of enemy activity in this area.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("Split up, I want to know what route they've taken to sneak past our lines.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(3f);

        ToastSystem.instance.SendDialogue("Understood, sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        yield return new WaitForSeconds(5f);

        //level complete, move onto the next one
        ToastSystem.instance.SendDialogue("Move out.",
        portraitLabel: "Archer", portrait: GlobalUnitManager.singleton.GetPortrait("Ranged").Item1, autoDismissTime: 5f);

        yield return new WaitForSeconds(5f);


        //load next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    private IEnumerator triggerReinforcements()
    {
        yield return new WaitForSeconds(reinforcementDelay);
        
        foreach (Transform child in reinforcements.transform)
        {
            if (child.TryGetComponent(out UnitAffiliation unitaff) && unitaff.affiliation != ControlSystem.instance.affiliation) {
                GlobalUnitManager.singleton.HideUnit(child.gameObject);
            }
            child.gameObject.SetActive(true);
        }
        GlobalUnitManager.singleton.Reindex();

        ToastSystem.instance.SendDialogue("Reinforcements have arrived!",
        portraitLabel: "Scout", portrait: GlobalUnitManager.singleton.GetPortrait("Scout").Item1, autoDismissTime: 5f);

        // yield return null;
    }

    int dialogueCounter = 0;
    void TickDialogue() {
        dialogueCounter++;
    }
}
