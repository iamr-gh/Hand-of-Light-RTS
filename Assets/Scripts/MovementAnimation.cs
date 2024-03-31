using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class MovementAnimation : MonoBehaviour
{
    //private SpriteRenderer spriteRenderer;
    private Rigidbody rb;
    private string facing = "Left";
    private bool isTurning = false;
    [SerializeField] float turnDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTurning) { return; } // Only start turning again after finishing once

        Debug.Log(facing + " | " + rb.velocity);
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x * -1, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        if (facing == "Left" && rb.velocity.x > 0)
        {
            facing = "Right";
            StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
        }
        else if(facing == "Right" && rb.velocity.x < 0)
        {
            facing = "Left";
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
