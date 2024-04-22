using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBattleScript : MonoBehaviour {
    public GameObject goal;
    // Start is called before the first frame update
    private ulong obj;
    void Start() {

        StartCoroutine(WaitForEnemiesDefeated());
    }

    int GetActiveChildCount() {
        int activeChildren = 0;
        for (int idx = 0; idx < transform.childCount; idx++) {
            GameObject child = transform.GetChild(idx).gameObject;
            if (child.activeSelf == true) {
                activeChildren++;
            }
        }

        return activeChildren;
    }

    IEnumerator WaitForEnemiesDefeated() {
        // Wait for all enemies to be defeated
        obj = ToastSystem.instance.SendObjective("Defeat all enemies");
        while (true) {
            if (GetActiveChildCount() <= 0) { break; }
            yield return null;
        }
        ToastSystem.instance.CompleteObjective(obj);
        StartCoroutine(win());

        goal.GetComponent<GoalScript>().SetAllEnemiesDefeated(true);
    }
    IEnumerator win() {
        ToastSystem.instance.AdvanceDialogue();

        ToastSystem.instance.SendDialogue("We did it!",
        portraitLabel: "Knight", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 6f, audioClip: Resources.Load<AudioClip>("Audio/Scout Lines/ScoutLine16"));

        ToastSystem.instance.SendDialogue("Victory is ours!",
        portraitLabel: "General", autoDismissTime: 5f, audioClip: Resources.Load<AudioClip>("Audio/general lines/General_19"));

        yield return new WaitForSeconds(12f);
        SceneManager.LoadScene(0);
    }
}
