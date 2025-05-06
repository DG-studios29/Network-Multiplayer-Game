using Mirror;
using UnityEngine;

public class KrakenHealth : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth = 100;
    public int maxHealth = 100;

    private Animator animator;

    public delegate void HealthChanged(int current, int max);
    public event HealthChanged OnHealthChangedEvent;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        if (currentHealth == 0)
        {
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
        if (animator != null)
        {
            animator.SetTrigger("isDead");
        }
    }
}
