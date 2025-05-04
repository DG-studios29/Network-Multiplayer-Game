using Mirror;
using UnityEngine;

public class KrakenHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    [Header("Health")]
    public int currentHealth = 100;
    public int maxHealth = 100;


    private Animator animator;
    public delegate void HealthChanged(int current, int max);
    public event HealthChanged OnHealthChangedEvent;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void OnHealthChanged(int oldValue, int newValue)
    {
        OnHealthChangedEvent?.Invoke(newValue, maxHealth);
    }

    void Die()
    {
        Debug.Log("Kraken Died");
        animator.SetTrigger("isDead");

    }
}
