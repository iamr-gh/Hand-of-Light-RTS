using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAspectSizer : MonoBehaviour {
    AspectRatioFitter fitter;

    // Start is called before the first frame update
    void Start() {
        TryGetComponent(out fitter);
    }

    // Update is called once per frame
    void Update() {
        fitter.aspectRatio = Camera.main.aspect;
    }
}
