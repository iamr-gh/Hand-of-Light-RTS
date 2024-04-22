using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LvRunAwayScript : MonoBehaviour
{
    public GameObject finalCheckpointEnemies;
    public GameObject friendlies;

    private bool escaped = false;
    private int timesAdvanced = 0;

    // Start is called before the first frame update
    void Awake() {
        StartCoroutine(dialogue());
    }

    private void OnTriggerEnter(Collider other) {
        UnitAffiliation affil = other.gameObject.GetComponent<UnitAffiliation>();
        if (affil != null && affil.affiliation == "White") {
            escaped = true;
        }
    }

    IEnumerator dialogue() {
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        //disable controls for dialogue
        input.actions.FindActionMap("Player").Disable();
        cam_move.enabled = false;

        ToastSystem.instance.onDialogueAdvanced.AddListener(TickDialogue);

        // Camera.main.transform.position = new Vector3(5.5f, 40f, 60f); // Move camera to correct spot

        ToastSystem.instance.SendDialogue("Sir! We've been spotted by enemy forces! A massive army is approaching our location!",
        portraitLabel: "Scout", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine11"));

        ToastSystem.instance.SendDialogue("We need to retreat as fast as we can. Are our commando units in position?",
        portraitLabel: "General", autoDismiss: false, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_14"));

        ToastSystem.instance.SendDialogue("Yes sir!",
        portraitLabel: "Scout", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false, autoDismissTime: 6f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine12"));

        ToastSystem.instance.SendDialogue("Reporting in!",
        portraitLabel: "Commando", portrait: GlobalUnitManager.singleton.GetPortrait("Commando").Item1, autoDismiss: false, audioClip: Resources.Load<AudioClip>("Audio/Commando Lines/AmirNassiri_CommandoLinesThree"));

        while (timesAdvanced < 4) { yield return null; } // Do nothing while dialogue hasn't finished

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        ulong aliveObjectiveID = ToastSystem.instance.SendObjective("Keep the scouts alive");
        ulong reachedObjectiveID = ToastSystem.instance.SendObjective("Rejoin the main army");

        ToastSystem.instance.CompleteObjective(aliveObjectiveID);
        //var notif6 = ToastSystem.instance.SendNotification("Press 1 to use a units ability.", false);
        //yield return new WaitForSeconds(2f);
        //yield return new WaitForSeconds(10f);
        //yield return new WaitForSeconds(4f);
        //ToastSystem.instance.DismissNotification(notif6);

        StartCoroutine(watchFriendlies(aliveObjectiveID)); // Start in background
        while (finalCheckpointEnemies.transform.childCount > 0 || !escaped) {
            yield return null;
        }

        ToastSystem.instance.CompleteObjective(reachedObjectiveID);

        //on win

        ToastSystem.instance.SendDialogue("We've made it!",
        portraitLabel: "Scout", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine13"));

        ToastSystem.instance.SendDialogue("Don't get too excited. We need to keep moving or they'll catch us. Let?s go!",
        portraitLabel: "General", autoDismissTime: 7f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_15"));

        yield return new WaitForSeconds(10f);

        ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

        //load next level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    IEnumerator watchFriendlies(ulong aliveObjectiveID) {
        while(friendlies.transform.childCount > 0) {
            yield return null;
        }
        ToastSystem.instance.UncompleteObjective(aliveObjectiveID);
    }

    void TickDialogue() { timesAdvanced += 1; }
}