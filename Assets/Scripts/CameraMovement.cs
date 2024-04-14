using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement
    public float edgeSize = 20f; // Size of the edge in pixels

    public float zoomSpeed = 5f; // Speed of camera zooming
    public float zoomSensitivity = 10f; // Sensitivity of zooming

    public float maxZoomDistance = 5f;
    public float maxMoveDist = 15.0f;
    
    private float initialx = 0f;
    private float initialz = 0f;

    public Transform targetObject; // Reference to the object to center the camera on
    private float initialZoomDistance; // Initial distance of the camera from the target object

    InputActionMap input;

    float currentZoomValue;

    // Start is called before the first frame update
    void Start()
    {
        if (targetObject != null) {
            initialZoomDistance = Vector3.Distance(transform.position, targetObject.position);
        } else {
            initialZoomDistance = Camera.main.transform.position.y;
        }
        currentZoomValue = 0f;
        input = GlobalUnitManager.singleton.GetComponent<PlayerInput>().actions.FindActionMap("Camera");
        
        initialx = transform.position.x;
        initialz = transform.position.z;

    }


    // Update is called once per frame
    void Update()
    {
        Vector3 cameraLocation = transform.position;
        // if (input.FindAction("Center Camera").WasPressedThisFrame())
        // {
        //     if (targetObject != null)
        //     {
        //         transform.position = targetObject.position - transform.forward * initialZoomDistance;
        //     }
        // }

        // Zoom in and out with scroll wheel
        float scrollInput = input.FindAction("Zoom Camera").ReadValue<float>();
        float zoomAmount = scrollInput * zoomSpeed * zoomSensitivity * Time.deltaTime;
        Vector3 zoomDirection = transform.forward;
        // zoomDirection = new Vector3(Mathf.Abs(zoomDirection.x), Mathf.Abs(zoomDirection.y), Mathf.Abs(zoomDirection.z));
        if ((zoomAmount > 0 && currentZoomValue < maxZoomDistance) || (zoomAmount < 0 && currentZoomValue > -maxZoomDistance)) {
            transform.position += zoomDirection * zoomAmount;
            currentZoomValue += zoomAmount;
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
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        var kbdInput = input.FindAction("Pan Camera").ReadValue<Vector2>();

        // Check for arrow key inputs
        if (kbdInput.x < 0 && (transform.position.x >= initialx-maxMoveDist))
        {
            mouseMovement -= cameraRight;
        }
        if (kbdInput.x > 0 && (transform.position.x <= initialx + maxMoveDist))
        {
            mouseMovement += cameraRight;
        }
        if (kbdInput.y > 0 && (transform.position.z <= initialz + maxMoveDist))
        {
            mouseMovement += cameraForward;
        }
        if (kbdInput.y < 0 && (transform.position.z >= initialz -maxMoveDist))
        {
            mouseMovement -= cameraForward;
        }

        // Find mouse inputs
        if (mousePosition.x <= edgeSize && (transform.position.x >= initialx-maxMoveDist))
        {
            mouseMovement -= cameraRight;
        }
        else if (mousePosition.x >= Screen.width - edgeSize && (transform.position.x <= maxMoveDist))
        {
            mouseMovement += cameraRight;
        }
        if (mousePosition.y <= edgeSize && (transform.position.z >= initialz-maxMoveDist))
        {
            mouseMovement -= cameraForward;
        }
        else if (mousePosition.y >= Screen.height - edgeSize && (transform.position.z <= initialz + maxMoveDist))
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
