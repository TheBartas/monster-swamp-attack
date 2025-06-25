using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : MonoBehaviour, IDamageable
{
    [Header("Features")] 
    [SerializeField] private float hp;
    private float maxHP;

    [Header("UI")] 
    [SerializeField] private Image hpBar;

    private void Start() {
        maxHP = hp;
    }
    public void Damage(float damage) {
        hp -= damage;
        hpBar.fillAmount = hp / maxHP;
        if (hp <= 0) {
            Debug.LogError("DOM ZNISZCZONY - ZRESETOWAĆ GRĘ!!!!"); // TO DO
            Destroy(gameObject);
        }
    }
}
