using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Image healthBarSprite;
    private Image background;
    private Camera camera;
    // [SerializeField] private Canvas canvasTransform;
    private RectTransform canvasTransform;

    private void Start() {
        camera = Camera.main;
        canvasTransform = GetComponentInChildren<Canvas>()?.GetComponent<RectTransform>();
    }

    private void Awake()
    {
        // canvasTransform = GetComponentInChildren<Canvas>();
        Image[] allImages = GetComponentsInChildren<Image>();
        foreach (Image image in allImages)
        {
            if (image.gameObject.name == "Background")
            {
                background = image;
            }
            if (image.gameObject.name == "Foreground") 
            {
                healthBarSprite = image;
                break;
            }
        }
    }

    private void Update() {
        canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - camera.transform.position);
    }

    public void UpdateEnemyHealthBar(float maxHp, float currentHp) {
        healthBarSprite.fillAmount = currentHp / maxHp;
    }

    public void KillBar() {
        background.enabled = false;
    }
}
