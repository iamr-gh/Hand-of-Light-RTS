using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobMovement : MonoBehaviour
{
    public float amp = 0.3f;
    public float freq = 3f;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(initPos.x, Mathf.Sin(Time.time * freq) * amp + initPos.y, initPos.z);
    }
}
