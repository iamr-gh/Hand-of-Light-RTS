using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Tutorial());
    }

    public IEnumerator Tutorial()
    {
        //give everything a frame to get started
        yield return null;
       ToastSystem.Instance.onRequest.Invoke("Hold and drag to box-select units",5f);
       ToastSystem.Instance.onRequest.Invoke("Right click to command your units to move!",5f);
       ToastSystem.Instance.onRequest.Invoke("Keep your weak units safe, and try to kill opponent's weak units",5f);
       ToastSystem.Instance.onRequest.Invoke("Press 'S' To Start", 5f);

    }

}
