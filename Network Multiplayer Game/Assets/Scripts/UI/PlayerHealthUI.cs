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
    [SerializeField] private GameObject playerHUD;

    [Header("Repair Settings")]
    [SerializeField] private float repairDuration = 5f;
    [SerializeField] private KeyCode repairKey = KeyCode.R;

    private bool isRepairing = false;
    private float repairTimer = 0f;
    private float repairProgress = 0f; 

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (playerHUD != null)
        {
            playerHUD.SetActive(true);
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no health bar assigned!");
        }
    }

    private void Start()
    {
        if (!isLocalPlayer && playerHUD != null)
        {
            playerHUD.SetActive(false);
        }

        if (isServer)
        {
            currentHealth = maxHealth;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

       
        if (currentHealth <= 0 && Input.GetKey(repairKey) && !isRepairing)
        {
            isRepairing = true;
            repairTimer = 0f;
            repairProgress = 0f; 
            Debug.Log("Repair started.");
        }

        // If repair key is released, stop repairing
        if (isRepairing && Input.GetKeyUp(repairKey))
        {
            isRepairing = false;
            repairTimer = 0f;
            repairProgress = 0f; 
            Debug.Log("Repair cancelled.");
        }

        if (isRepairing)
        {
            repairTimer += Time.deltaTime;
            repairProgress = Mathf.Clamp01(repairTimer / repairDuration); 

            // This is just for visuals, not actual health value.
            if (healthBar != null)
            {
                healthBar.value = Mathf.Lerp(currentHealth, maxHealth, repairProgress);
            }

            if (repairTimer >= repairDuration)
            {
                isRepairing = false;
                repairTimer = 0f;
                Debug.Log("Repair completed, sending to server...");
                CmdRequestRepair();
            }
        }
    }

    [Command]
    private void CmdRequestRepair()
    {
        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} repaired on server.");
            currentHealth = maxHealth;
            RpcNotifyRepair();
        }
    }

    [ClientRpc]
    private void RpcNotifyRepair()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Repair successful. Health restored.");
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

        Debug.Log($"{gameObject.name} took {damage} damage. Now at {currentHealth}/{maxHealth}.");
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        Debug.Log($"{gameObject.name} health changed: {oldHealth} → {newHealth}");

        if (healthBar != null)
        {
            healthBar.value = newHealth;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no health bar assigned!");
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
}
