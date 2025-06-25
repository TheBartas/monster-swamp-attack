using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;
    private float healthTime = 1.0f; 
    private float currentEnergyRate = 2.5f; 
    private float healthRechargeDelay = 2.0f;

    private bool isRecharging = false;
    private Coroutine rechargeCoroutine = null;

    [Header("UI")]
    [SerializeField] private Image hpForeground;

    private DeadMenu deadMenu;

    void Start()
    {
        deadMenu = GetComponent<DeadMenu>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
        hpForeground.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StopRecharge(); 
            Invoke(nameof(StartRecharge), healthRechargeDelay); // wznowi po opóźnieniu
        }
    }

    private void StopRecharge()
    {
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
            rechargeCoroutine = null;
            isRecharging = false;
        }
    }

    private void StartRecharge()
    {
        if (!isRecharging && currentHealth < maxHealth)
        {
            rechargeCoroutine = StartCoroutine(RechargeHP());
        }
    }

    private IEnumerator RechargeHP()
    {
        isRecharging = true;
        yield return new WaitForSeconds(healthTime); 

        while (currentHealth < maxHealth)
        {
            currentHealth += currentEnergyRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth); 
            hpForeground.fillAmount = currentHealth / maxHealth;

            yield return null;
        }

        isRecharging = false;
        rechargeCoroutine = null; 
    }

    private void Die()
    {
        Debug.Log("Gracz zginął");
        deadMenu.ShowMenu();
    }

    public void AddHealth(float amount) {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
        hpForeground.fillAmount = currentHealth / maxHealth; 

        if (currentHealth == maxHealth) {
            StopRecharge(); 
        }
    }
}
