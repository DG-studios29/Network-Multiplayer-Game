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

    private void Start()
    {
        currentHealth = maxHealth;

        SphereCollider myCollider = GetComponent<SphereCollider>();
        myCollider.radius = 100f;
        myCollider.isTrigger = true;

        // Optional: log for debugging
        Debug.Log("Island initialized with " + cannons.Count + " cannons.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered island radius.");
            playerNearby = true;

            if (isDestroyed && !isLooted)
            {
                StartCoroutine(CollectTreasureRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log("Player left island radius.");
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
            StartCoroutine(DespawnAfterDelay());
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log("Island took damage. Current HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("Island destroyed! Stay nearby to collect treasure.");
            // Optional: play destruction FX here
        }
    }

    public void LootIsland()
    {
        isLooted = true;
        Debug.Log("Island looted!");
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
