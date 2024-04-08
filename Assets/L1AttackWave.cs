using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class L1AttackWave : MonoBehaviour
{
    public bool autoStart = false;
    private bool started = false;
    public Vector3 amove_location;
    
    
    void Update(){
       if(!started && autoStart){
           StartWave();
           started = true;
       }
    }
    
    public void StartWave()
    {
        Debug.Log("Child count: " + transform.childCount);
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
            Debug.Log("Child: " + child.name);
            if(child.gameObject.TryGetComponent(out UnitAI unitAI))
            {
                unitAI.AttackMoveToCoordinate(amove_location);
            }
        }
    }
}
