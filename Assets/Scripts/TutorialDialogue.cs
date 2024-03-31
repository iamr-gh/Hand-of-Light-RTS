using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogue : MonoBehaviour
{
    // bool started = false;
    public int level = 1;
    ToastSystem toastSystem;

    ulong tutorialNotificationId;

    void Start()
    {
        toastSystem = ToastSystem.Instance;
        tutorialNotificationId = toastSystem.SendNotification("Press \"T\" for tutorial", autoDismiss: false);
        toastSystem.SendNotification("I was sent second!", priority: NotificationPriority.High, autoDismissTime: 2f);
        toastSystem.SendNotification("I was sent third!", priority: NotificationPriority.Low, autoDismissTime: 6f);
        toastSystem.SendNotification("I was sent fourth!", priority: NotificationPriority.Medium, autoDismissTime: 4f);
        if (level == 1 && Input.GetKeyDown(KeyCode.T)) { StartCoroutine(TutorialLevel1()); }
        else if (level == 2) { StartCoroutine(TutorialLevel2()); }
        else if (level == 3) { StartCoroutine(TutorialLevel3()); }
        else if (level == 4) { StartCoroutine(TutorialLevel4()); }
    }

    void Update()
    {
        if (level == 1 && Input.GetKeyDown(KeyCode.T)) { StartCoroutine(TutorialLevel1()); }
    }

    public IEnumerator TutorialLevel1()
    {
        //give everything a frame to get started
        toastSystem.DismissNotification(tutorialNotificationId);
        yield return null;
        toastSystem.SendDialogue("Bump the edges of the screen with your mouse to move camera");
        toastSystem.SendDialogue("Hold and drag to box-select units");
        toastSystem.SendDialogue("Right click to command your units to move!");
        // toastSystem.SendDialogue("You can also double click units to select all friendlies of that type");
        // toastSystem.SendDialogue("Control-click a friendly unit does the same thing");
        toastSystem.SendDialogue("These units Are Warriors, they are slow but powerful");
        toastSystem.SendDialogue("Defeat your opponent and gather the gold!");
    }

    public IEnumerator TutorialLevel2()
    {
        //give everything a frame to get started
        yield return null;
        toastSystem.SendDialogue("Now you know how command your army");
        toastSystem.SendDialogue("We've supplemented your forces with some archers in blue");
        toastSystem.SendDialogue("Use your range superiority to defeat the enemy and get the gold!");
        yield return new WaitForSeconds(23.0f);// not fifo, so weird stuff if under 9s
        toastSystem.SendDialogue("Keep your archers back to keep them alive");
    }

    public IEnumerator TutorialLevel3()
    {
        //give everything a frame to get started
        yield return null;
        toastSystem.SendDialogue("That chokepoint looks dangerous");
        toastSystem.SendDialogue("Especially with all those ranged units in the back");
        toastSystem.SendDialogue("Let's try to find another way to deal with them", autoDismissTime: 5f);
        yield return new WaitForSeconds(15.0f);
        toastSystem.SendDialogue("We've give you some calvary in white to help");
    }

    public IEnumerator TutorialLevel4(){
        yield return null;
        toastSystem.SendDialogue("Use the skills you've learned to defeat your opponent");
    }
}
