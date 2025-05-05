using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NetworkIdentity))]
public class Island : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] public Slider healthSlider;
    [SerializeField] public Slider lootSlider;
    [SerializeField] private GameObject healthBarUI;
    [SerializeField] private GameObject lootBarUI;
    [SerializeField] private float lootBarDelay = 2f;
    public LootRadiusVisualizer lootRadiusVisualizer;

    [HideInInspector] public IslandManager manager;
    [HideInInspector] public GameObject islandPrefab;

    [SyncVar] public bool isLooted = false;
    [SyncVar] private bool isDestroyed = false;

    [Header("Island Settings")]
    public float maxHealth = 10f;
    [SyncVar(hook = nameof(OnHealthChanged))] private float currentHealth;

    [Header("Treasure Settings")]
    public float treasureCollectionTime = 5f;

    [Header("Defences")]
    public List<CannonDefence> cannons;

    private Coroutine lootCoroutine;
    private HashSet<GameObject> playersInside = new HashSet<GameObject>();
    private float currentLootProgress = 0f;

    private SphereCollider myCollider;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;

        myCollider = GetComponent<SphereCollider>();
        myCollider.radius = 100f;
        myCollider.isTrigger = true;
    }

    void Start()
    {
        if (lootBarUI != null) lootBarUI.SetActive(false);
        if (healthBarUI != null) healthBarUI.SetActive(true);

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }


    private void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (lootSlider != null)
        {
            lootSlider.gameObject.SetActive(playersInside.Count == 1 && isDestroyed && !isLooted);
            lootSlider.value = currentLootProgress / treasureCollectionTime;
        }
    }

    private void OnHealthChanged(float oldHealth, float newHealth)
    {
        currentHealth = newHealth;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside.Add(other.gameObject);

            // Notify LootRadiusVisualizer that the player is inside
            LootRadiusVisualizer radiusVisualizer = GetComponentInChildren<LootRadiusVisualizer>();
            if (radiusVisualizer != null)
            {
                radiusVisualizer.SetPlayerInside(true);
            }

            if (isDestroyed && !isLooted && playersInside.Count == 1 && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }


    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside.Remove(other.gameObject);

            // Notify LootRadiusVisualizer that the player is no longer inside
            LootRadiusVisualizer radiusVisualizer = GetComponentInChildren<LootRadiusVisualizer>();
            if (radiusVisualizer != null)
            {
                radiusVisualizer.SetPlayerInside(false);
            }

            if (lootCoroutine != null)
            {
                StopCoroutine(lootCoroutine);
                lootCoroutine = null;
                currentLootProgress = 0f;
            }

            if (isDestroyed && !isLooted)
            {
                StartCoroutine(WaitBeforeDespawn());

                if (playersInside.Count == 1)
                {
                    lootCoroutine = StartCoroutine(CollectTreasureRoutine());
                }
            }
        }
    }

    [Server]
    private IEnumerator WaitBeforeDespawn()
    {
        yield return new WaitForSeconds(10f);

        if (playersInside.Count == 0)
        {
            StartCoroutine(DespawnAfterDelay());
        }
    }

    [Server]
    private IEnumerator CollectTreasureRoutine()
    {
        currentLootProgress = 0f;

        while (playersInside.Count == 1 && currentLootProgress < treasureCollectionTime)
        {
            currentLootProgress += Time.deltaTime;
            yield return null;
        }

        if (currentLootProgress >= treasureCollectionTime)
        {
            LootIsland();
            StartCoroutine(DespawnAfterDelay());
        }

        lootCoroutine = null;
    }

    [Server]
    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        Debug.Log($"Island took damage: {amount}. Current health: {currentHealth}");

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("Island destroyed");

            if (healthBarUI != null)
                healthBarUI.SetActive(false);

            if (lootBarUI != null)
                StartCoroutine(ShowLootBarWithDelay());

            if (playersInside.Count == 1 && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }


    [Server]
    public void LootIsland()
    {
        isLooted = true;

        int lootAmount = Random.Range(100, 1001);

        // Get the player who looted
        GameObject player = playersInside.FirstOrDefault();
        if (player != null)
        {
            // Try to get ScoreboardManager
            ScoreboardManager scoreboard = player.GetComponent<ScoreboardManager>();
            if (scoreboard != null)
            {
                scoreboard.CmdIncreaseScore(lootAmount);
            }


            //TargetShowLootPopup(player.GetComponent<NetworkIdentity>().connectionToClient, lootAmount);
        }

        RpcNotifyLootCollected(lootAmount);
    }

    [TargetRpc]
    private void TargetShowLootPopup(NetworkConnection target, int amount)
    {
        Debug.Log($"[UI] You looted {amount} gold!");
        LootPopupManager.Instance?.ShowLootPopup(amount);

    }


    [ClientRpc]
    private void RpcNotifyLootCollected(int amount)
    {
        Debug.Log("Island looted! Loot collected: " + amount);
    }

    [Server]
    private IEnumerator DespawnAfterDelay()
    {
        Debug.Log("Despawn coroutine started");

        manager?.NotifyIslandDespawn(this);

        yield return new WaitForSeconds(5f);

        if (manager != null && islandPrefab != null)
        {
            Debug.Log("Spawning new island...");
            manager.SpawnIsland(islandPrefab);
        }
        else
        {
            Debug.LogWarning("Island prefab or manager missing!");
        }

        if (isServer)
        {
            Debug.Log("Destroying island on server...");
            NetworkServer.Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Tried to destroy island from a client!");
        }
    }


    private IEnumerator ShowLootBarWithDelay()
    {
        yield return new WaitForSeconds(lootBarDelay);

        if (lootBarUI != null)
        {
            lootBarUI.SetActive(true);
            Debug.Log("Loot bar displayed.");
        }
    }

}