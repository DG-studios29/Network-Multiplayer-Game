using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerHealthUI : NetworkBehaviour
{
    [Header("Health Settings")]
    [SyncVar(hook = nameof(OnHealthChanged))] 
    public int currentHealth;

    public int maxHealth = 100;

    [Header("UI")]
    [SerializeField] private Slider healthBar; 

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

       
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Start()
    {
        if (isServer)
        {
            
            currentHealth = maxHealth;
        }
    }

    [Server]
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcHandleDeath();
        }

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}/{maxHealth}");
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (isLocalPlayer && healthBar != null)
        {
            healthBar.value = newHealth;
        }
    }

    [ClientRpc]
    private void RpcHandleDeath()
    {
        if (isLocalPlayer)
        {
            Debug.Log("You died!");
            
        }
    }
    

       private void Update()
    {

        /* Damage testing code (uncomment to test damage)
        if (Input.GetKeyDown(KeyCode.Space) && currentHealth != null)
        {
            TakeDamage(damage: 10);
        }
        */
    }
}