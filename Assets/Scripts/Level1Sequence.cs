using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Sequence : MonoBehaviour
{
    bool step1Camera = false;
    var input = GlobalUnitManager.singleton.getcomponent<PlayerInput>;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        yield return new WaitForSeconds(3f);
    }
}
