using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseOnNoFriendlies : MonoBehaviour
{
    //this is really inefficient
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(EndIfNoFriendlies()); 
    }
    
    IEnumerator EndIfNoFriendlies(){
        yield return null;
        while(true){
            yield return null;
            var allManaged = GameObject.FindGameObjectsWithTag("Managed");
            bool liveFriendlies = false;
            foreach(var obj in allManaged){
                if(obj.TryGetComponent(out UnitAffiliation aff)){
                    if(aff.affiliation == "White"){
                        liveFriendlies = true;
                        break;
                    }
                }
            }
            
            if(!liveFriendlies){
                ToastSystem.instance.SendNotification("All Units Died, Resetting...", autoDismissTime: 5f);
                yield return new WaitForSeconds(5);
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // Reload current scene
            }
        }
    }

}
