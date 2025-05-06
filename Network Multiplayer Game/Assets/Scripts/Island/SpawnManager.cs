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
    public List<Transform> playerSpawnPoints;  
    public List<Transform> islands;            
    public List<Transform> rocks;              
    public float avoidBigIslandRadius = 150f;
    public float avoidObjectRadius = 100f;     

  
    public override void OnStartServer()
    {
        base.OnStartServer();
        Instance = this;
    }

    [Server]
    public bool IsPositionAvailable(Vector3 position)
    {
        // Check against occupied positions
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }

        // Check against player spawn points
        foreach (Transform playerSpawn in playerSpawnPoints)
        {
            if (Vector3.Distance(position, playerSpawn.position) < avoidObjectRadius)
                return false;
        }

        // Check against existing island positions
        foreach (Transform island in islands)
        {
            if (Vector3.Distance(position, island.position) < avoidObjectRadius)
                return false;
        }

        // Check against existing rock positions
        foreach (Transform rock in rocks)
        {
            if (Vector3.Distance(position, rock.position) < avoidObjectRadius)
                return false;
        }

        // Avoid the big island
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

      
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (Transform spawn in playerSpawnPoints)
        {
            Gizmos.DrawWireSphere(spawn.position, 50);
        }

        if (bigIsland != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bigIsland.position, avoidBigIslandRadius);
        }

        
    
        Gizmos.color = Color.red;
        foreach (Vector3 pos in occupiedPositions)
        {
            Gizmos.DrawWireSphere(pos, minDistanceBetweenObjects / 2f);
        }
    }
}


