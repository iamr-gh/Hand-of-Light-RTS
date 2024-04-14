using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        yield return null;
        var input = GlobalUnitManager.singleton.GetComponent<PlayerInput>();
        var cam_move = Camera.main.GetComponent<cameraMovement>();
        var controlSystem = GlobalUnitManager.singleton.GetComponent<ControlSystem>();

        ToastSystem.instance.SendDialogue("Commander, we must win! ajskldaklsdjalksjdkalsjdlkasjdlkasjdlkasdjalksjdehfgskreghu sah ahsjdhjaselkf huahe ulhaelf haeukdf ", portrait: GlobalUnitManager.singleton.GetPortrait("Melee").Item1, portraitLabel: "Knight", autoDismiss: false, blur: true);
        ToastSystem.instance.SendNotification("Use your new skills to defeat the enemy army.", true, 5f);
        var nid = ToastSystem.instance.SendNotification("Use your new skills to defeat the enemy army.", autoDismiss: false, boxColor: Color.red);
        var id = ToastSystem.instance.SendObjective("Win!");
        yield return new WaitForSeconds(3f);
        ToastSystem.instance.CompleteObjective(id);
        ToastSystem.instance.DismissNotification(nid);
    }
}
