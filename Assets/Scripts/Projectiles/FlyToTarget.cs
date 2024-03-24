using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyToTarget : MonoBehaviour
{
    public GameObject target;
     public float speed = 10.0f;
     public float rangeToDestroy = 0.5f;
     private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
       TryGetComponent(out rb) ;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null){
            Destroy(gameObject);
        }
        else{
            var offset = target.transform.position - transform.position;
            if((offset).magnitude < rangeToDestroy){
                Destroy(gameObject);
            }
            else{
                rb.velocity = offset.normalized*speed;
            }
        }
    }
    
    void LateUpdate(){
       //  
    }
}
