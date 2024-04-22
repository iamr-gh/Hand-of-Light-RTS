using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MountainPassEndDialogue : MonoBehaviour {
    public GameObject chasers;
    public GameObject friendlies;
    private bool started = false;
    private List<GameObject> inTrigger = new();
    private int numInitialFriendlies;


    ulong objectiveId;
    void Start() {
        numInitialFriendlies = friendlies.transform.childCount;
        StartCoroutine(AddObjective());
    }

    IEnumerator AddObjective() {
        yield return null;
        objectiveId = ToastSystem.instance.SendObjective("Keep at least half the units alive");
    }


    void OnTriggerEnter(Collider other) {
        // check if unit is friendly
        if (!started && other.TryGetComponent(out UnitAffiliation unitAffiliation) && unitAffiliation.affiliation == "White") {
            inTrigger.Add(other.gameObject);
            if (inTrigger.Count == friendlies.transform.childCount) {
                started = true;
                chasers.SetActive(false);
                StartCoroutine(dialogue());
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (!started && other.TryGetComponent(out UnitAffiliation unitAffiliation) && unitAffiliation.affiliation == "White") {
            inTrigger.Remove(other.gameObject);
        }
    }

    bool failed = false;
    void Update() {
        inTrigger.RemoveAll(obj => obj == null);
        if (!started && inTrigger.Count == friendlies.transform.childCount) {
            started = true;
            chasers.SetActive(false);
            StartCoroutine(dialogue());
        }
        if (!failed && friendlies.transform.childCount < numInitialFriendlies / 2) {
            ToastSystem.instance.FailObjective(objectiveId);
            failed = true;
        }
    }

    IEnumerator dialogue() {
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;
        var (commandoPortrait, commandoColor) = GlobalUnitManager.singleton.GetPortrait("Commando");

        if (friendlies.transform.childCount >= numInitialFriendlies / 2) {
            ToastSystem.instance.onDialogueAdvanced.AddListener(TickDialogue);

            ToastSystem.instance.CompleteObjective(objectiveId);

            ToastSystem.instance.SendDialogue("We made it!",
            portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 6f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine13"));

            ToastSystem.instance.SendDialogue("Don’t get too excited. We need to keep moving or they’ll catch us. Let’s go!",
            portraitLabel: "General", autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_15"));
            //ToastSystem.instance.SendDialogue("We made it!",
            //portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, portraitLabel: "Knight", autoDismissTime: 5f);
            //// yield return new WaitForSeconds(5f);

            //ToastSystem.instance.SendDialogue("We won't forget those who fell this day. Let's get out of here.",
            //portrait: commandoPortrait, portraitColor: commandoColor, portraitLabel: "Commando", autoDismissTime: 5f);
            // yield return new WaitForSeconds(5f);
            dialogueTicked = 0;
            while (dialogueTicked < 2) {
                yield return null;
            }
            ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

            //advance level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else {
            ToastSystem.instance.onDialogueAdvanced.AddListener(TickDialogue);

            ToastSystem.instance.SendDialogue("We lost too many men. The enemy has bested us.",
            portrait: commandoPortrait, portraitColor: commandoColor, portraitLabel: "Commando", autoDismissTime: 5f);

            dialogueTicked = 0;
            while (dialogueTicked < 1) {
                yield return null;
            }
            ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

            //restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    int dialogueTicked = 0;
    void TickDialogue() {
        dialogueTicked++;
    }
}
