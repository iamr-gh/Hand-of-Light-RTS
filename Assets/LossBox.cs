using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LossBox : MonoBehaviour
{
    bool triggered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out UnitAffiliation aff) && aff.affiliation != "White")
        {
            if (!triggered)
            {
                triggered = true;
                StartCoroutine(end());
            }


        }
    }

    IEnumerator end()
    {
        ToastSystem.Instance.SendDialogue("We failed General, they breached the pass.",
        portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, autoDismissTime: 5f);
        yield return new WaitForSeconds(3f);
        
        
        ToastSystem.Instance.SendNotification("Resetting Level...");
        yield return new WaitForSeconds(2f);
        //reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
