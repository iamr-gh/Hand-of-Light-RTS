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
    void Start() {
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

        Camera.main.transform.position = new Vector3(5.5f, 40f, 60f); // Move camera to correct spot

        ToastSystem.instance.SendDialogue("Commander! Our scouts have found the enemy forces. Unfortunately, they also found us.",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);

        ToastSystem.instance.SendDialogue("There's some commandos coming in later that will help you to move between the islands",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);

        ToastSystem.instance.SendDialogue("Get the scouts out and link up with the main army!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismiss: false);

        while (timesAdvanced < 3) { yield return null; } // Do nothing while dialogue hasn't finished

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

        ToastSystem.instance.SendDialogue("We've reached the main army, the intelligence gathered today will be critical in finding good terrain to engage them on",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Finally, we'll be able to destroy their main army!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 3f);

        yield return new WaitForSeconds(5f);

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