using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NetworkIdentity))]
public class Island : NetworkBehaviour
{
    [HideInInspector] public IslandManager manager;
    [HideInInspector] public GameObject islandPrefab;

    [SyncVar] public bool isLooted = false;
    [SyncVar] private bool isDestroyed = false;

    private bool playerNearby = false;

    [Header("Island Settings")]
    public float maxHealth = 100f;
    [SyncVar] private float currentHealth;

    [Header("Treasure Settings")]
    public float treasureCollectionTime = 5f;

    [Header("Defences")]
    public List<CannonDefence> cannons;

    private Coroutine lootCoroutine;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;

        SphereCollider myCollider = GetComponent<SphereCollider>();
        myCollider.radius = 100f;
        myCollider.isTrigger = true;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (isDestroyed && !isLooted && lootCoroutine == null)
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
            playerNearby = false;

            if (isDestroyed && !isLooted && lootCoroutine != null)
            {
                StopCoroutine(lootCoroutine);
                lootCoroutine = null;
                StartCoroutine(WaitBeforeDespawn());
            }
        }
    }

    [Server]
    private IEnumerator WaitBeforeDespawn()
    {
        yield return new WaitForSeconds(10f);

        if (!playerNearby)
        {
            StartCoroutine(DespawnAfterDelay());
        }
    }

    [Server]
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

            if (playerNearby && lootCoroutine == null)
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
        RpcNotifyLootCollected(lootAmount);
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
