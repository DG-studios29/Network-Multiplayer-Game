using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.InputSystem;

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

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction repairAction;
    private bool isRepairing = false;
    private float repairTimer = 0f;
    private float repairProgress = 0f;

    private void Awake()
    {
        if (inputActions != null)
        {
            var map = inputActions.FindActionMap("Player", true);
            repairAction = map.FindAction("Repair", true);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} is missing InputActionAsset!");
        }
    }

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

        if (repairAction != null)
        {
            repairAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (isLocalPlayer && repairAction != null)
        {
            repairAction.Disable();
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
        if (!isLocalPlayer || repairAction == null) return;

        if (currentHealth <= 0 && repairAction.IsPressed() && !isRepairing)
        {
            isRepairing = true;
            repairTimer = 0f;
            repairProgress = 0f;
            Debug.Log("Repair started.");
        }

        if (isRepairing && !repairAction.IsPressed())
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

    [Command]
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        RpcTakeDamage();
        OnHealthChanged(currentHealth, currentHealth - damage);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            RpcHandleDeath();
        }

        
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        

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

    [ClientRpc]
    private void RpcTakeDamage()
    {
        if (isLocalPlayer)
        {
            if (currentHealth <= 0) return;

            currentHealth -= 10;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                RpcHandleDeath();
            }

            if (healthBar != null)
            {
                healthBar.value = currentHealth;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} has no health bar assigned!");
            }

            
        }
    }
}
