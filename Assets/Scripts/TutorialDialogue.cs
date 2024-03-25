using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogue : MonoBehaviour
{
    bool started = false;
    public int level = 1;

    void Update(){
        if(level == 1 && Input.GetKeyDown(KeyCode.T)) { StartCoroutine(TutorialLevel1()); }
        else if(level == 2) { StartCoroutine(TutorialLevel2()); }
        else if(level == 3) { StartCoroutine(TutorialLevel3()); }
    }

    public IEnumerator TutorialLevel1()
    {
        //give everything a frame to get started
        yield return null;
        ToastSystem.Instance.onRequest.Invoke("Bump the edges of the screen with your mouse to move camera",3f);
        ToastSystem.Instance.onRequest.Invoke("Hold and drag to box-select units",3f);
        ToastSystem.Instance.onRequest.Invoke("Right click to command your units to move!",3f);
        ToastSystem.Instance.onRequest.Invoke("You can also double click units to select all friendlies of that type",3f);
        ToastSystem.Instance.onRequest.Invoke("Control-click a friendly unit does the same thing", 3f);
        ToastSystem.Instance.onRequest.Invoke("Select your Melee Attackers and go find the gold!",3f);
    }

    public IEnumerator TutorialLevel2() {
        //give everything a frame to get started
        yield return null;
        ToastSystem.Instance.onRequest.Invoke("Now you know how command your army", 3f);
        ToastSystem.Instance.onRequest.Invoke("We've supplemented your forces with some archers in blue", 3f);
        ToastSystem.Instance.onRequest.Invoke("Use them to destroy the enemies' superior numbers and get the gold!", 3f);
    }

    public IEnumerator TutorialLevel3() {
        //give everything a frame to get started
        yield return null;
        ToastSystem.Instance.onRequest.Invoke("That chokepoint looks dangerous", 3f);
        ToastSystem.Instance.onRequest.Invoke("Especially with all those ranged units in the back", 3f);
        ToastSystem.Instance.onRequest.Invoke("Let's try to find another way to deal with them", 3f);
    }

}
