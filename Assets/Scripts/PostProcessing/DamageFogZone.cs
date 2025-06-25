using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class DamageFogZone : MonoBehaviour
{
    [Header("Damage Settings")]
    public float dmg;

    [Header("GUI Effects")]
    public float vignetteIntensity; // Maksymalna intensywność efektu
    public float effectFadeSpeed; // Szybkość narastania i zanikania efektu
    public float blurIntensity; // Maksymalna intensywność rozmycia
    public float blurFadeSpeed; // Szybkość narastania i zanikania rozmycia

    private Vignette vignette;
    private float currentIntensity = 0f; 
    private PostProcessVolume volume; 


    private DepthOfField depthOfField;
    private float currentBlur = 0f;


    private void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        if (volume.profile.TryGetSettings(out vignette) && volume.profile.TryGetSettings(out depthOfField))
        {
            depthOfField.focusDistance.value = 0f; 
            depthOfField.aperture.value = 0.1f; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dmg * Time.deltaTime);
                float healthProportion = 1f - ((float) playerHealth.currentHealth / (float) playerHealth.maxHealth);
                if (vignette != null)
                {
                    currentIntensity = Mathf.Lerp(currentIntensity, healthProportion * vignetteIntensity, effectFadeSpeed * Time.deltaTime);
                    vignette.intensity.value = currentIntensity;
                }

                if (depthOfField != null)
                {
                    currentBlur = Mathf.Lerp(currentBlur, healthProportion * blurIntensity, blurFadeSpeed * Time.deltaTime);
                    depthOfField.aperture.value = Mathf.Lerp(0.1f, 0.2f, currentBlur); 
                    depthOfField.focusDistance.value = Mathf.Lerp(0.1f, 2f, currentBlur); 
                }
            }

        }
        else
        {
            if (vignette != null)
            {
                currentIntensity = Mathf.Lerp(currentIntensity, 0f, effectFadeSpeed * Time.deltaTime);
                vignette.intensity.value = currentIntensity;
            }


            if (depthOfField != null)
            {
                currentBlur = Mathf.Lerp(currentBlur, 0f, blurFadeSpeed * Time.deltaTime);
                depthOfField.aperture.value = Mathf.Lerp(0.1f, 2f, currentBlur);
                depthOfField.focusDistance.value = Mathf.Lerp(0.1f, 2f, currentBlur);
            }
        }
    }
}
