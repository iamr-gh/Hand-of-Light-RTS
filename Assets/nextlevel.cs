using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextlevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(win());
        
    }

    IEnumerator win()
    {
        ToastSystem.Instance.AdvanceDialogue();
        ToastSystem.Instance.SendDialogue("Level Complete.", autoDismiss: false);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
