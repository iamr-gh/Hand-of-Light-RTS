using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(UnitParameters))]

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private UnitParameters parameters;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parameters = GetComponent<UnitParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        float healthRatio = parameters.getHP() / parameters.maxHP;
        transform.localScale = new Vector3(healthRatio, transform.localScale.y, transform.localScale.z);
        if (healthRatio <= 0.2) {
            spriteRenderer.color = Color.red;
        }
        else if (healthRatio <= 0.5) {
            spriteRenderer.color = Color.yellow;
        }
        else {
            spriteRenderer.color = Color.green;
        }
    }
}
