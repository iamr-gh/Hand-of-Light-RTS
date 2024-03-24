using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTIme : MonoBehaviour {
    public float time = 0.5f;
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, time);
    }
}
