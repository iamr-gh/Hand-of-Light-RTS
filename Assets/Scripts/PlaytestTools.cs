using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
//R to reload the level, G to reset the whole game, 
//number keys to reset to particular level?
public class PlaytestTools : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
        if(Input.GetKeyDown(KeyCode.Semicolon)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        if(Input.GetKeyDown(KeyCode.Quote)){
            SceneManager.LoadScene(0);
        }
    }
}
