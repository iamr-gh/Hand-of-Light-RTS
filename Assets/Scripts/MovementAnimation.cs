using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SpriteRenderer))]

public class MovementAnimation : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private string facing = "Left";
    private bool isTurning = false;
    [SerializeField] float turnDuration = 0.2f;
    [SerializeField] float hopHeight = 0.2f;

    private float moveStartTime;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponentInParent<NavMeshAgent>();
        moveStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Have character hop

        if (navAgent.velocity.magnitude < 0.5f) {
            moveStartTime = Time.time + Random.Range(0f, 0.3f);
        }
        else {
            float positionInCycle = Mathf.Abs(Mathf.Sin(2 * Mathf.PI * (Time.time - moveStartTime)));
            transform.position = transform.parent.position + hopHeight * transform.up * positionInCycle;
            //transform.RotateAround(transform.position, transform.forward, hopRotation * (positionInCycle - prevRotationAngle));
            //prevRotationAngle = Mathf.Sin(2 * Mathf.PI * (Time.time - moveStartTime));
            //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y,
            //                                      hopRotation * Mathf.Sin(Time.time % (2 * Mathf.PI)));
        }
        if (isTurning) { return; } // Only start turning again after finishing once

        if (facing == "Left" && navAgent.velocity.x > 0)
        {
            facing = "Right";

            StartCoroutine(RotateObject(transform.up, 180f, turnDuration));
            //Quaternion targetRotation = Quaternion.Euler(-30, 180, 0);
            //StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
        }
        else if(facing == "Right" && navAgent.velocity.x < 0)
        {
            facing = "Left";
            StartCoroutine(RotateObject(transform.up, 180f, turnDuration));
            //Quaternion targetRotation = Quaternion.Euler(30, 0, 0);
            //StartCoroutine(RotateObject(transform.rotation, targetRotation, turnDuration));
        }
    }

    IEnumerator RotateObject(Vector3 axis, float angle, float duration_sec) {
        isTurning = true;
        float initialTime = Time.time;
        float angularVelocity = angle / duration_sec;
        float deltaAngle = 0;
        while (Mathf.Abs(Time.time - initialTime) < duration_sec) {
            transform.RotateAround(transform.position, axis, angularVelocity * Time.deltaTime);
            deltaAngle += angularVelocity * Time.deltaTime;
            yield return null;
        }
        transform.RotateAround(transform.position, transform.up, angle - deltaAngle);

        isTurning = false;
    }

    IEnumerator RotateObject(Quaternion initialQuaternion, Quaternion targetQuaternion, float duration_sec)
    {
        isTurning = true;
        float initial_time = Time.time;
        float progress = 0;
        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / duration_sec;
            Quaternion newRotation = Quaternion.Lerp(initialQuaternion, targetQuaternion, progress);
            transform.rotation = newRotation;
            yield return null;
        }
        transform.rotation = targetQuaternion;
        isTurning = false;
    }
}
