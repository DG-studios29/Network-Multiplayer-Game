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
    [SyncVar] private bool isDespawning = false;
    [SyncVar] private float syncedLootProgress;


    [Header("Island Settings")]
    public float maxHealth = 30f;
    [SyncVar(hook = nameof(OnHealthChanged))] private float currentHealth;
    [SyncVar(hook = nameof(OnRadiusVisibilityChanged))] private bool showRadius = false;

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
            healthSlider.maxValue = maxHealth;            
            healthSlider.value = currentHealth;           
        }
    }


    private void OnRadiusVisibilityChanged(bool _old, bool newVal)
    {
        if (lootRadiusVisualizer != null)
        {
            lootRadiusVisualizer.SetVisible(newVal);
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (other.CompareTag("Player"))
        {
            playersInside.Add(other.gameObject);

            LootRadiusVisualizer radiusVisualizer = GetComponentInChildren<LootRadiusVisualizer>();
            if (radiusVisualizer != null)
            {
                radiusVisualizer.SetVisible(true);
            }
            if (!showRadius) showRadius = true;
            if (isDestroyed && !isLooted && playersInside.Count == 1 && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }


    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;
        if (other.CompareTag("Player"))
        {
            playersInside.Remove(other.gameObject);
            

            LootRadiusVisualizer radiusVisualizer = GetComponentInChildren<LootRadiusVisualizer>();
            if (radiusVisualizer != null)
            {
                radiusVisualizer.SetVisible(false);
            }

            if (playersInside.Count == 0 && !isDespawning && isDestroyed)
            {
                showRadius = false;
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

                isDespawning = true;
                StartCoroutine(DespawnAfterDelay());
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

    [ServerCallback]
    private IEnumerator CollectTreasureRoutine()
    {
        currentLootProgress = 0f;

        while (playersInside.Count == 1 && currentLootProgress < treasureCollectionTime)
        {
            currentLootProgress += Time.deltaTime;
            syncedLootProgress = currentLootProgress; // sync value to all clients
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
        GameObject player = playersInside.FirstOrDefault();
        Debug.Log($"[SERVER] LootIsland: Found player = {player?.name}");
        if (player != null)
        {
            // Try to get ScoreboardManager
            ScoreboardManager scoreboard = player.GetComponent<ScoreboardManager>();
            if (scoreboard != null)
            {
                scoreboard.CmdIncreaseScore(lootAmount);
            }

        }

        RpcNotifyLootCollected(lootAmount);
    }

    [ClientRpc]
    private void RpcNotifyLootCollected(int amount)
    {


        var player = NetworkClient.localPlayer;
        if (player != null)
        {
            var ui = player.GetComponent<PlayerIslandUI>();
            if (ui != null)
            {
                Debug.Log("Updating loot collected UI");
                ui.ShowUI(true);
                ui.UpdateLootCollected(amount);
            }
            else
            {
                Debug.LogWarning("No PlayerIslandUI found!");
            }
        }
        Debug.Log("Island looted! Loot collected: " + amount);
    }

    [Server]
    private IEnumerator DespawnAfterDelay()
    {
        Debug.Log("Despawn coroutine started");
        yield return new WaitForSeconds(5f);

        if (playersInside.Count == 0)
        {
            manager?.NotifyIslandDespawn(this); 
            Debug.Log("Spawning new island...");
            if (manager != null && islandPrefab != null)
            {
                manager.SpawnIsland(islandPrefab);
            }

            if (isServer)
            {
                Debug.Log("Destroying island on server...");
                NetworkServer.Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Players returned before despawn. Canceling.");
            isDespawning = false;
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

    public float GetCurrentHealth() => currentHealth;
    public float GetLootProgress()
    {
        return syncedLootProgress / treasureCollectionTime;
    }
    
}