using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//GPT 4 aided in the writing of this

//message, duration
[System.Serializable]
public class ToastMessageEvent : UnityEvent<string, float> { }
public class ToastSystem : MonoBehaviour
{
    public ToastMessageEvent onRequest;

    public static ToastSystem Instance;
    private TMP_Text display; //could eventually convert into a game object that lives and dies for prettier things
    private bool active = false; //we literally have an async mutex

    //I don't need to store the queue myself, it will auto manage?
    // public List<ToastMessageEvent> 
    // Start is called before the first frame update
    void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        }
        else {
            Instance = this;
            if(onRequest == null){
                onRequest = new ToastMessageEvent();
            }
        }
    }
    void Start()
    {
        TryGetComponent(out display);
        display.text = "";

    }

    public void HandleMsg(string msg, float duration)
    {
        StartCoroutine(TempDisplay(msg,duration));
    }

    private IEnumerator TempDisplay(string msg, float duration)
    {
        //could change this to an explicit wait list, but a spin lock is fine for rn
        while(active){
            yield return null;
        }

        //it's kinda dumb bc if we need an internal queue, why use event system
        active = true;

        display.text = msg;
        yield return new WaitForSeconds(duration);
        display.text = "";
        active = false;
    }

    private void OnEnable(){
        onRequest.AddListener(HandleMsg);
    }    

    private void OnDisable(){
        onRequest.RemoveListener(HandleMsg);
    }    

}
