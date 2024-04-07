using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

public class Level4Sequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    // Update is called once per frame
    IEnumerator tutorial()
    {
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        ToastSystem.Instance.SendDialogue("Commander, we must win!", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1);
        ToastSystem.Instance.SendNotification("Use your new skills to defeat the enemy army.", NotificationPriority.Low, true, 5f);
        yield return new WaitForSeconds(3f);
    }
}
