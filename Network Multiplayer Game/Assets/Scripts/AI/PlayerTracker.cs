using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerTracker : NetworkBehaviour
{
    [Header("Tracking Settings")]
    public float searchInterval = 2f;
    public string playerTag = "Player";

    [HideInInspector] public Transform currentTarget;
    [HideInInspector] public PlayerHealthUI playerHealth;

    private float lastSearchTime = -Mathf.Infinity;

    private void Update()
    {
        if (!isServer) return;

        if (Time.time - lastSearchTime >= searchInterval || currentTarget == null || playerHealth == null)
        {
            lastSearchTime = Time.time;
            FindClosestPlayer();
        }
    }

    private void FindClosestPlayer()
    {
        var players = NetworkServer.spawned
            .Where(kv => kv.Value != null && kv.Value.CompareTag(playerTag))
            .Select(kv => kv.Value)
            .ToArray();

        if (players.Length == 0)
        {
            
            currentTarget = null;
            playerHealth = null;
            return;
        }

        var closest = players.OrderBy(p => Vector3.Distance(transform.position, p.transform.position)).FirstOrDefault();
        if (closest != null)
        {
            currentTarget = closest.transform;
            playerHealth = closest.GetComponent<PlayerHealthUI>();
            
        }
    }
}
