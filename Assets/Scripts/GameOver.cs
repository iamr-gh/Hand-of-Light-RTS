using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameOver : MonoBehaviour
{
    public string message;
    
    public int deadCount = 0;
    [SerializeField] int minimumAliveFriendlies = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForEnemiesDefeated());
    }

    int GetActiveChildCount()
    {
        int activeChildren = 0;
        for (int idx = 0; idx < transform.childCount; idx++) {
            GameObject child = transform.GetChild(idx).gameObject;
            if(child.activeSelf == true) {
                activeChildren++;
            }
        }

        return activeChildren;
    }

    IEnumerator WaitForEnemiesDefeated()
    {
        // Wait for all enemies to be defeated
        while (true)
        {
            if (GetActiveChildCount() <= minimumAliveFriendlies) { break; }
            yield return null;
        }

        // Lock inputs
        GlobalUnitManager.singleton.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        Camera.main.GetComponent<cameraMovement>().enabled = false;

        // Print a Message
        ToastSystem.instance.SendDialogue(message, autoDismissTime: 3f);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }
}
