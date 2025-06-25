using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireFence_36_v1 : Barrier
{
    private float dmg = 20;
    private float attackCooldown = 4.0f;
    private bool canAttack = false;

    private void Start()
    {
        health = 750f;
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && !canAttack)
        {
            canAttack = true;
            StartCoroutine(AttackEnemy(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            canAttack = false;
            StopAllCoroutines(); // Zatrzymanie pętli ataku
        }
    }

    private IEnumerator AttackEnemy(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            yield break;
        }

        while (canAttack && other != null)
        {
            if (other == null) 
            {
                yield break;
            }
            damageable.Damage(dmg);
            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
