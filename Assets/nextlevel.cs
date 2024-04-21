using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class nextlevel : MonoBehaviour
{
    public UnityEvent onAdvancing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        onAdvancing.Invoke();
        StartCoroutine(win());
        
    }

    IEnumerator win()
    {
        ToastSystem.instance.AdvanceDialogue();
        ToastSystem.instance.SendDialogue("Level Complete.", autoDismiss: false);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
