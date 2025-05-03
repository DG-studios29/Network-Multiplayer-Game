using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Island : NetworkBehaviour
{
    [HideInInspector] public IslandManager manager;
    [HideInInspector] public GameObject islandPrefab;

    private bool playerNearby = false;
    [SyncVar(hook = nameof(OnDestroyedChanged))] private bool isDestroyed = false;
    [SyncVar(hook = nameof(OnLootedChanged))] private bool isLooted = false;

    [Header("Island Settings")]
    public float maxHealth = 100f;
    [SyncVar(hook = nameof(OnHealthChanged))] private float currentHealth;

    [Header("Treasure Settings")]
    public float treasureCollectionTime = 5f;

    [Header("Defences")]
    public List<CannonDefence> cannons;

    private Coroutine lootCoroutine;

    private void Start()
    {
        // Initialize health on server
        if (isServer) currentHealth = maxHealth;

        SphereCollider myCollider = GetComponent<SphereCollider>();
        myCollider.radius = 100f;
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player entered island radius.");
        playerNearby = true;

        if (isDestroyed && !isLooted && lootCoroutine == null)
        {
            lootCoroutine = StartCoroutine(CollectTreasureRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        Debug.Log("Player left island radius.");

        if (isDestroyed && !isLooted && lootCoroutine != null)
        {
            StopCoroutine(lootCoroutine);
            lootCoroutine = null;
            StartCoroutine(WaitBeforeDespawn());
        }
    }

    [Server]
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log($"Island took damage. Current HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("Island destroyed! Stay nearby to loot.");

            if (playerNearby && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }

    private IEnumerator WaitBeforeDespawn()
    {
        yield return new WaitForSeconds(10f);

        if (!playerNearby)
        {
            Debug.Log("Player didn't stay nearby to loot. Despawning island...");
            yield return DespawnAndRespawn();
        }
    }

    private IEnumerator CollectTreasureRoutine()
    {
        float timer = 0f;

        while (playerNearby && timer < treasureCollectionTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer >= treasureCollectionTime)
        {
            LootIsland();
            yield return DespawnAndRespawn();
        }

        lootCoroutine = null;
    }

    [Server]
    private IEnumerator DespawnAndRespawn()
    {
        manager?.NotifyIslandDespawn(this);
        yield return new WaitForSeconds(5f);

        if (manager != null && islandPrefab != null)
        {
            manager.SpawnIsland(islandPrefab);
        }

        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void LootIsland()
    {
        if (isLooted) return;
        isLooted = true;
        Debug.Log("Island looted!");

        int lootAmount = Random.Range(100, 1001);
        Debug.Log($"Loot collected: {lootAmount}");

        RpcOnLoot(lootAmount);
    }

    #region SyncVar Hooks & RPCs

    private void OnHealthChanged(float oldValue, float newValue)
    {
        // Update health UI or progress bars here
    }

    private void OnDestroyedChanged(bool oldValue, bool newValue)
    {
        // Trigger destruction VFX/logic on clients
    }

    private void OnLootedChanged(bool oldValue, bool newValue)
    {
        // Show looted state (e.g., change material) on clients
    }

    [ClientRpc]
    private void RpcOnLoot(int amount)
    {
        // Client-side loot effect, e.g., display popup
    }

    #endregion
}
