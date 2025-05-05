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
    public Slider healthSlider;
    public Slider lootSlider;
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

            if (isDestroyed && !isLooted && lootCoroutine == null)
            {
                if (playersInside.Count == 1)
                {
                    Debug.Log("Start looting island is destroyed");
                    lootCoroutine = StartCoroutine(CollectTreasureRoutine());
                }
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
        isLooted = true;
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
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("island Destroyed");
            if (playersInside.Count == 1 && lootCoroutine == null)
            {
                Debug.Log("Player inside on destruction — starting loot coroutine.");
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

          
            TargetShowLootPopup(player.GetComponent<NetworkIdentity>().connectionToClient, lootAmount);
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
        manager?.NotifyIslandDespawn(this);
        yield return new WaitForSeconds(5f);

        if (manager != null && islandPrefab != null)
        {
            manager.SpawnIsland(islandPrefab);
        }

        NetworkServer.Destroy(gameObject);
    }
}
