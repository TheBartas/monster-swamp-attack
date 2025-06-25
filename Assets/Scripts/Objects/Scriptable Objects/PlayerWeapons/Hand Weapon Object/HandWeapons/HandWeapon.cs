using System.Collections;
using UnityEngine;
using TMPro;

public class HandWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider weapon; // Obiekt broni (z Colliderm)
    [SerializeField] private HandWeaponData handWeaponData; // Dane broni (obrażenia itp.)
    [SerializeField] private TextMeshProUGUI ammunitionDisplay; // Wyświetlacz (opcjonalnie)

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.4f; 
    private float lastAttackTime = 0f;
    private Animator animator;

    private void Start()
    {
        UpdateAmmoDisplay(); 
        weapon.enabled = false;
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown) {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            weapon.enabled = true;
            StartCoroutine(ResetAttackCooldown());
        }
    }

    private IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        weapon.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.Damage(handWeaponData.dmg);
            }
            Debug.Log("Uderzenie w przeciwnika!");
        }
    }

    public void UpdateAmmoDisplay()
    {
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.text = string.Empty;
        } 
    }
}
