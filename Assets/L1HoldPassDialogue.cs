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
    public GameObject fourthWave;
    
    //maybe should include another wave

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
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 7f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine1"));

        //make general speaker
        ToastSystem.instance.SendDialogue("What are we looking at exactly?",
        //using ranged for general rn
        portraitLabel: "General", autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_1"));

        ToastSystem.instance.SendDialogue("A contingent of knights. No other support. Though, some of their advanced party have captured several villagers.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 7f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine2"));

        ToastSystem.instance.SendDialogue("Those bastards...",
        portraitLabel: "General", autoDismissTime: 12f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_20"));

        ToastSystem.instance.SendDialogue("Right. Our first priority is to hold the mountain passes until reinforcements arrive.",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("We rescue the villagers when we get the opportunity. Move out!",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Sir yes Sir!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine3"));

        while (dialogueCounter < 7) {
            yield return null;
        }

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;
        
        var wav1obj = ToastSystem.instance.SendObjective("Hold the mountain pass! (Wave 1/4)");

        //have a reinforcment condition on timer
        StartCoroutine(triggerReinforcements());

        //start triggering enemy waves
        //at least after 2 waves of stacking
        var awave1 = firstWave.GetComponent<L1AttackWave>();
        awave1.start = true;

        yield return new WaitForSeconds(15f);

        ToastSystem.instance.RemoveObjective(wav1obj);
        var wav2obj = ToastSystem.instance.SendObjective("Hold the mountain pass! (Wave 2/4)");
        var awave2 = secondWave.GetComponent<L1AttackWave>();
        awave2.start = true;
        
        //after this, wait for defeat for stacking

        while (!awave1.defeated)
        {
            yield return null;
        }
        while (!awave2.defeated)
        {
            yield return null;
        }
        
        //could make reinforcements not be timed based, but that seems wrong?

        ToastSystem.instance.RemoveObjective(wav2obj);
        var wav3obj = ToastSystem.instance.SendObjective("Hold the mountain pass! (Wave 3/4)");
        var awave3 = thirdWave.GetComponent<L1AttackWave>();
        awave3.start = true;
        
        while (!awave3.defeated)
        {
            yield return null;
        }

        ToastSystem.instance.RemoveObjective(wav3obj);
        var obj = ToastSystem.instance.SendObjective("Hold the mountain pass! (Wave 4/4)");
        var awave4 = fourthWave.GetComponent<L1AttackWave>();
        awave4.start = true;

        while (!awave4.defeated)
        {
            yield return null;
        }

        ToastSystem.instance.CompleteObjective(obj);
        dialogueCounter = 0;
        //on win

        ToastSystem.instance.SendDialogue("Enemy forces have been routed sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine4"));

        ToastSystem.instance.SendDialogue("Good. Now, we need to find out where this force came from.",
        portraitLabel: "General", autoDismissTime: 13f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_2"));

        ToastSystem.instance.SendDialogue("We've received no reports of enemy activity in this area.",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Split up, I want to know what route they've taken to sneak past our lines.",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Understood, sir.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine5"));

        //level complete, move onto the next one
        ToastSystem.instance.SendDialogue("Move out.",
        portraitLabel: "General", autoDismissTime: 4f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_3"));

        while (dialogueCounter < 6) {
            yield return null;
        }

        ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);


        //load next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    private IEnumerator triggerReinforcements()
    {
        yield return new WaitForSeconds(reinforcementDelay);
        
        foreach (Transform child in reinforcements.transform)
        {
            if (!(child.TryGetComponent(out UnitAffiliation unitaff) && unitaff.affiliation == ControlSystem.instance.affiliation)) {
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
