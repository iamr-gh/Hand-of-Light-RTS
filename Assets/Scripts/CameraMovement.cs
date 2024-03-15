using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement
    public float edgeSize = 20f; // Size of the edge in pixels

    public float zoomSpeed = 5f; // Speed of camera zooming
    public float zoomSensitivity = 10f; // Sensitivity of zooming

    public Transform targetObject; // Reference to the object to center the camera on
    private float initialZoomDistance; // Initial distance of the camera from the target object

    // Start is called before the first frame update
    void Start()
    {
        initialZoomDistance = Vector3.Distance(transform.position, targetObject.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraLocation = transform.position;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (targetObject != null)
            {
                transform.position = targetObject.position - transform.forward * initialZoomDistance;
            }
        }

        // Zoom in and out with scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scrollInput * zoomSpeed * zoomSensitivity * Time.deltaTime;
        Vector3 zoomDirection = transform.forward;
        zoomDirection = new Vector3(Mathf.Abs(zoomDirection.x), Mathf.Abs(zoomDirection.y), Mathf.Abs(zoomDirection.z));
        if (zoomAmount > 0 && transform.position.y <= 10) {
            transform.position += zoomDirection * zoomAmount;
        }
        if (zoomAmount < 0 && transform.position.y >= 2)
        {
            transform.position += zoomDirection * zoomAmount;
        }

        // Get the camera's forward and right directions
        Vector3 cameraForward = transform.forward;
        Vector3 cameraRight = transform.right;

        // Project the camera directions onto the game space
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Check for mouse position
        Vector3 mouseMovement = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        // Check for arrow key inputs
        if (Input.GetKey(KeyCode.LeftArrow) && (transform.position.x >= -10f))
        {
            mouseMovement -= cameraRight;
        }
        if (Input.GetKey(KeyCode.RightArrow) && (transform.position.x <= 10f))
        {
            mouseMovement += cameraRight;
        }
        if (Input.GetKey(KeyCode.UpArrow) && (transform.position.z <= 10f))
        {
            mouseMovement += cameraForward;
        }
        if (Input.GetKey(KeyCode.DownArrow) && (transform.position.z >= -10f))
        {
            mouseMovement -= cameraForward;
        }

        // Find mouse inputs
        if (mousePosition.x <= edgeSize && (transform.position.x >= -10f))
        {
            mouseMovement -= cameraRight;
        }
        else if (mousePosition.x >= Screen.width - edgeSize && (transform.position.x <= 10f))
        {
            mouseMovement += cameraRight;
        }
        if (mousePosition.y <= edgeSize && (transform.position.z >= -10f))
        {
            mouseMovement -= cameraForward;
        }
        else if (mousePosition.y >= Screen.height - edgeSize && (transform.position.z <= 10f))
        {
            mouseMovement += cameraForward;
        }

        // Normalize the mouse movement direction
        if (mouseMovement.magnitude > 1)
        {
            mouseMovement.Normalize();
        }

        // Move the camera parallel to the game space based on the mouse movement direction

        transform.Translate(mouseMovement * moveSpeed * Time.deltaTime, Space.World);

    }
}
