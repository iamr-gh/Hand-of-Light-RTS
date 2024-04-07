using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class HealthBarCanvas : MonoBehaviour {
    private Slider healthSlider;
    private Image healthBarImage;
    private UnitParameters parameters;

    // Start is called before the first frame update
    void Start() {
        healthSlider = GetComponentInChildren<Slider>();
        healthBarImage = healthSlider.fillRect.gameObject.GetComponent<Image>();
        parameters = GetComponentInParent<UnitParameters>();
        transform.rotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update() {
        float healthRatio = parameters.getHP() / parameters.maxHP;
        healthSlider.SetValueWithoutNotify(healthRatio);
        if (healthRatio <= 0.2) {
            healthBarImage.color = Color.red;
        } else if (healthRatio <= 0.5) {
            healthBarImage.color = Color.yellow;
        } else {
            healthBarImage.color = Color.green;
        }
    }
}
