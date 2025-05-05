using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Control")]
    public float minDistanceBetweenObjects = 50f;
    public List<Vector3> occupiedPositions = new List<Vector3>();

    [Header("Avoid These (can be networked Transforms)")]
    public Transform bigIsland;
    public List<Transform> playerSpawnPoints;  // List of predefined spawn points for players
    public List<Transform> islands;            // List of island positions to avoid
    public List<Transform> rocks;              // List of rocks positions to avoid
    public float avoidBigIslandRadius = 150f;
    public float avoidObjectRadius = 100f;     // Distance to avoid rocks, islands, and players

    #region Server Initialization
    public override void OnStartServer()
    {
        base.OnStartServer();
        // Only the server ever needs this instance
        Instance = this;
    }
    #endregion

    #region Server-Only Spawn Logic
    [Server]
    public bool IsPositionAvailable(Vector3 position)
    {
        // Check against all previously spawned objects
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }

        // Check distance to any islands
        foreach (Transform island in islands)
        {
            if (Vector3.Distance(position, island.position) < avoidObjectRadius)
                return false;
        }

        // Check distance to any rocks
        foreach (Transform rock in rocks)
        {
            if (Vector3.Distance(position, rock.position) < avoidObjectRadius)
                return false;
        }

        // Check distance to the big island (if assigned)
        if (bigIsland != null && Vector3.Distance(position, bigIsland.position) < avoidBigIslandRadius)
            return false;

        return true;
    }

    [Server]
    public void RegisterPosition(Vector3 position)
    {
        occupiedPositions.Add(position);
    }

    [Server]
    public void UnregisterPosition(Vector3 position)
    {
        occupiedPositions.Remove(position);
    }

    // Spawn players at predefined spawn points
    [Server]
    public Transform GetAvailableSpawnPoint()
    {
        foreach (Transform spawnPoint in playerSpawnPoints)
        {
            if (IsPositionAvailable(spawnPoint.position))
            {
                RegisterPosition(spawnPoint.position);
                return spawnPoint;
            }
        }

        // If no spawn points are available, return null or handle as needed
        return null;
    }
    #endregion
}
