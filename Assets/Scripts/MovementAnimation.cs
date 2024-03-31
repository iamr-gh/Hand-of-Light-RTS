using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class MovementAnimation : MonoBehaviour
{
    private Rigidbody rb;
    private string facing = "Left";
    private bool isTurning = false;
    [SerializeField] float turnDuration = 0.2f;
    [SerializeField] float hopHeight = 0.2f;
    [SerializeField] float hopRotation = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Have character hop
        //if(rb.velocity.magnitude != 0)
        //{
        //    
        //    transform.position = new Vector3(transform.position.x,
        //                                     hopHeight * Mathf.Abs(Mathf.Sin(Time.time % (2 * Mathf.PI))),
        //                                     transform.position.z);
        //    transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y,
        //                                          hopRotation * Mathf.Sin(Time.time % (2 * Mathf.PI)));
        //}
        if (isTurning) { return; } // Only start turning again after finishing once

        if (facing == "Left" && rb.velocity.x > 0)
        {
            facing = "Right";
            Quaternion targetRotation = Quaternion.Euler(-30, 180, 0);
            StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
        }
        else if(facing == "Right" && rb.velocity.x < 0)
        {
            facing = "Left";
            Quaternion targetRotation = Quaternion.Euler(30, 0, 0);
            StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
        }
    }

    IEnumerator RotateObject(Quaternion initialQuaternion, Quaternion targetQuaternion, float duration_sec)
    {
        Debug.Log("StartedRotating!");
        isTurning = true;
        float initial_time = Time.time;
        float progress = 0;
        while (progress < 1.0f) {
            progress = (Time.time - initial_time) / duration_sec;
            Quaternion newRotation = Quaternion.Lerp(initialQuaternion, targetQuaternion, progress);
            transform.rotation = newRotation;
            yield return null;
        }
        transform.rotation = targetQuaternion;
        isTurning = false;
    }
}
