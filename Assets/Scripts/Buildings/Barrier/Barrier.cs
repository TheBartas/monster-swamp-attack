using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Barrier : MonoBehaviour, IDamageable
{
    protected float health = 0f;

    public virtual void Damage(float amount)
    {
        health -= amount;
        // Debug.Log($"HP:  {health}");
        if (health <= 0f)
        {
            gameObject.tag = "Dead";
            Destroy(gameObject, 0.1f); // Zniszczenie obiektu, gdy zdrowie spadnie do zera
        }
    }
}
