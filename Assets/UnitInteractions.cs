using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitInteractions : MonoBehaviour
{
    public float damageFlashPeriod = 0.1f;
    UnitParameters parameters;
    SpriteRenderer spriteRenderer;
    Color originalColor;

    private void Start()
    {
        parameters = GetComponent<UnitParameters>();
        for (int childIdx = 0; childIdx < transform.childCount; childIdx++)
        {
            GameObject child = transform.GetChild(childIdx).gameObject;
            if (child.name == "RelativeRenderer")
            {
                spriteRenderer = child.GetComponent<SpriteRenderer>();
            }
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    public void TakeDamage(float damage)
    {
        parameters.setHP(parameters.getHP() - damage);
        StartCoroutine(AttackFeedback());
    }
    IEnumerator AttackFeedback()
    {
        if (spriteRenderer != null) { spriteRenderer.color = Color.red; }
        yield return new WaitForSeconds(damageFlashPeriod);
        if (spriteRenderer != null) { spriteRenderer.color = originalColor; }
    }
}
