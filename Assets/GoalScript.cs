using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    public bool end = false;
    bool allEnemiesDefeated = false;
    bool nextLevelStarted = false;
    // Start is called before the first frame update
    public void SetAllEnemiesDefeated(bool defeated) { allEnemiesDefeated = defeated; }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidedWith = collision.gameObject;
        if (collidedWith.TryGetComponent(out UnitAffiliation aff) && aff.affiliation == "White"
            && allEnemiesDefeated && !nextLevelStarted) {
            nextLevelStarted = true;
            StartCoroutine(GoToNextLevel());
        }
    }

    IEnumerator GoToNextLevel() {
        AudioClip levelComplete = Resources.Load<AudioClip>("Audio/LevelComplete");
        AudioManager.instance.PlayAudioClip(levelComplete);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
