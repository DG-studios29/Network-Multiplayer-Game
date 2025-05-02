using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Island : MonoBehaviour
{
    [HideInInspector] public IslandManager manager;
    [HideInInspector] public GameObject islandPrefab;

    public bool isLooted = false;
    private bool playerNearby = false;
    private bool isDestroyed = false;

    [Header("Island Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Treasure Settings")]
    public float treasureCollectionTime = 5f;

    [Header("Defences")]
    public List<CannonDefence> cannons;

    private Coroutine lootCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;

        SphereCollider myCollider = GetComponent<SphereCollider>();
        myCollider.radius = 100f;
        myCollider.isTrigger = true;

      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered island radius.");
            playerNearby = true;

           
            if (isDestroyed && !isLooted && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log("Player left island radius.");

           
            if (isDestroyed && !isLooted && lootCoroutine != null)
            {
                StopCoroutine(lootCoroutine);
                lootCoroutine = null;
                StartCoroutine(WaitBeforeDespawn());
            }
        }
    }

    private IEnumerator WaitBeforeDespawn()
    {
        yield return new WaitForSeconds(10f); 

        if (!playerNearby) // Only despawn if the player is still not in range
        {
            Debug.Log("Player didn't stay nearby to loot. Despawning island...");
            StartCoroutine(DespawnAfterDelay());
        }
    }

    private IEnumerator CollectTreasureRoutine()
    {
        float timer = 0f;

        // Collect treasure only if player is still nearby
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

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log("Island took damage. Current HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("Island destroyed! Stay nearby to loot.");

            // If player is already nearby, start looting right away
            if (playerNearby && lootCoroutine == null)
            {
                lootCoroutine = StartCoroutine(CollectTreasureRoutine());
            }
        }
    }

    public void LootIsland()
    {
        isLooted = true;
        Debug.Log("Island looted!");

        int lootAmount = Random.Range(100, 1001);  
        Debug.Log("Loot collected: " + lootAmount);
    }

    private IEnumerator DespawnAfterDelay()
    {
        manager?.NotifyIslandDespawn(this);
        yield return new WaitForSeconds(5f);

        if (manager != null && islandPrefab != null)
        {
            manager.SpawnIsland(islandPrefab);
        }

        Destroy(gameObject);
    }
}
