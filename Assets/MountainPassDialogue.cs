using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MountainPassDialogue : MonoBehaviour {
    public GameObject chasers;
    // Start is called before the first frame update
    void Start() {
        chasers.SetActive(false);
        StartCoroutine(dialogue());
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

        ToastSystem.instance.SendDialogue("Sir! Enemy snipers line the mountains!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 6f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine14"));

        ToastSystem.instance.SendDialogue("Damn it. And the enemy army is right on our trail.",
        portraitLabel: "General", autoDismissTime: 15f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_16"));

        ToastSystem.instance.SendDialogue("We have no choice but to move through this area to regroup with our main army.",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("We can't afford to lose more than half our men for the next battle. Commandos, mages. You're up!",
        portraitLabel: "General", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Yes sir!",
        portraitLabel: "Super Commando", portraitColor: Color.yellow, portrait: GlobalUnitManager.singleton.GetPortrait("Commando").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Commando Lines/AmirNassiri_CommandoLinesFour"));

        ToastSystem.instance.SendDialogue("I read you.",
        portraitLabel: "Mage", portrait: GlobalUnitManager.singleton.GetPortrait("Mage").Item1, autoDismissTime: 3f, audioClip: Resources.Load<AudioClip>("Audio/Tutorial and Unit Lines/Unit Selected Line 4-[AudioTrimmer.com]"));

        //ToastSystem.instance.SendDialogue("General, we're trapped on this mountain face! Our reports say this area is covered in powerful sniper nests!",
        //portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, portraitLabel: "Knight", autoDismissTime: 5f);

        //ToastSystem.instance.SendDialogue("We need to make it across without losing more than half our men! The enemy will close in at any second!",
        //portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, portraitLabel: "Knight", autoDismissTime: 5f);

        //var (commandoPortrait, commandoColor) = GlobalUnitManager.singleton.GetPortrait("Commando");
        //ToastSystem.instance.SendDialogue("Don't worry! I can take a lot of damage and shepard our units across.",
        //portrait: commandoPortrait, portraitColor: Color.yellow, portraitLabel: "Super Commando", autoDismissTime: 5f);

        //var (magePortrait, mageColor) = GlobalUnitManager.singleton.GetPortrait("Mage");
        //ToastSystem.instance.SendDialogue("Remember last time? I can deal with any sniper nests.",
        //portrait: magePortrait, portraitLabel: "Mage", autoDismissTime: 5f);

        //ToastSystem.instance.SendDialogue("If we can make it to the green fog at the end of the pass, it should obscure us from the enemy!",
        //portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, portraitLabel: "Knight", autoDismissTime: 5f);

        ToastSystem.instance.SendDialogue("Get ready to run for your life!", autoDismiss: false);

        bool sentNoti = false;
        ulong prompt = 0;
        while (timesAdvanced < 7) {
            yield return null;
            if (!sentNoti && timesAdvanced == 6) {
                prompt = ToastSystem.instance.SendNotification("Press space to begin!", autoDismissTime: 5f);
                sentNoti = true;

            }
        }

        ToastSystem.instance.DismissNotification(prompt);

        ToastSystem.instance.onDialogueAdvanced.RemoveListener(TickDialogue);

        input.actions.FindActionMap("Player").Enable();
        cam_move.enabled = true;

        yield return new WaitForSeconds(1f);
        chasers.SetActive(true);

    }

    int timesAdvanced = 0;
    void TickDialogue() {
        timesAdvanced++;
    }

}
