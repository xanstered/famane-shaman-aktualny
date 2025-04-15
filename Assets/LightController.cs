using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light interiorLight;        
    public float transitionDuration = 0.5f;    
    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;

            if (interiorLight != null)
                StartCoroutine(FadeLight(interiorLight, 0, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;

            if (interiorLight != null)
                StartCoroutine(FadeLight(interiorLight, 0.5f, 0));
        }
    }

    private System.Collections.IEnumerator FadeLight(Light light, float startIntensity, float targetIntensity)
    {
        float elapsedTime = 0;
        while (elapsedTime < transitionDuration)
        {
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        light.intensity = targetIntensity;
    }

}
