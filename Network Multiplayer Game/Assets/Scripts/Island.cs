using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SphereCollider))]
public class Island : MonoBehaviour
{
    [HideInInspector] public IslandManager manager;
    [HideInInspector] public GameObject islandPrefab;

    public bool isLooted = false;
    private bool playerNearby = false;

    private void Start()
    {
        SphereCollider myCollider = transform.GetComponent<SphereCollider>();
        myCollider.radius = 20f;
        myCollider.isTrigger = true;
    }

    // Called when any Collider enters this island's trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("YES");
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            // If player leaves after looting, start despawn timer
            LootIsland();
            Debug.Log("NO");
            if (isLooted)
            {
                StartCoroutine(DespawnAfterDelay());
            }
        }
    }

    // Call this when player successfully loots the island
    public void LootIsland()
    {
        isLooted = true;
    }

    // After delay, despawn this island and request a respawn
    private IEnumerator DespawnAfterDelay()
    {
        // Let the manager know we're despawning (remove old pos)
        manager.NotifyIslandDespawn(this);

        // Wait for a few seconds before despawning&#8203;:contentReference[oaicite:16]{index=16}
        yield return new WaitForSeconds(5f);

        // Spawn a replacement island of the same type
        if (manager != null && islandPrefab != null)
        {
            manager.SpawnIsland(islandPrefab);
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Island GameObject already destroyed before despawn.");
        }
    }
}
