using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpireWithTime : MonoBehaviour
{
    public float timeToLive = 5.0f;
    private MeshRenderer meshRenderer;
    //could even have fading juice
    void Start()
    {
    TryGetComponent(out meshRenderer);
       StartCoroutine(Expire()); 
    }
    
    IEnumerator Expire()
    {
        yield return null;
        yield return null;
        
        if(meshRenderer == null){
            yield return new WaitForSeconds(timeToLive);
        }
        else{
            var end_mat = new Material(meshRenderer.material);
            // set transparency of material to 0
            var start_color = meshRenderer.material.color;
            var end_color = new Color(end_mat.color.r, end_mat.color.g, end_mat.color.b, 0f);

            //lerp over duration
            var elapsedTime = 0.0f;
            while (elapsedTime < timeToLive)
            {
                meshRenderer.material.color = Color.Lerp(start_color, end_color, elapsedTime / timeToLive);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        Destroy(gameObject);
    }

}
