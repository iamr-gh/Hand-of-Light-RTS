using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyToTarget : MonoBehaviour
{
    public GameObject target;
    public float speed = 10.0f;
    public float rangeToDestroy = 0.5f;
    public float projectileDamage;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
       TryGetComponent(out rb) ;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) { Destroy(gameObject); }
        else {
            var offset = target.transform.position - transform.position;
            if((offset).magnitude < rangeToDestroy) {
                // Deal damage then destroy projectile
                WeaponSystem weaponSystem = target.GetComponent<WeaponSystem>();
                //UnitInteractions targetInteractions = target.GetComponent<UnitInteractions>();
                if (weaponSystem.juice != null)
                {
                    weaponSystem.juice.TakeDamage(projectileDamage);
                }

                Destroy(gameObject);
            }
            else {
                transform.LookAt(target.transform); // Rotate to face the target
                rb.velocity = offset.normalized*speed;
            }
        }
    }
}
