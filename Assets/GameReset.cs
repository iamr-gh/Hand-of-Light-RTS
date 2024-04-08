using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject goal;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForEnemiesDefeated());
    }

    int GetActiveChildCount()
    {
        int activeChildren = 0;
        for (int idx = 0; idx < transform.childCount; idx++)
        {
            GameObject child = transform.GetChild(idx).gameObject;
            if (child.activeSelf == true)
            {
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
            if (GetActiveChildCount() <= 0) { break; }
            yield return null;
        }
        // ToastSystem.Instance.SendDialogue("Congratulations, you have completed the game!", autoDismissTime: 5.0f);
        yield return new WaitForSeconds(5.0f);

        SceneManager.LoadScene(0);
    }
}
