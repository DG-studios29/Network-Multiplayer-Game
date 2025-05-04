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
    public Transform player;
    public Transform bigIsland;
    public float avoidPlayerRadius = 100f;
    public float avoidBigIslandRadius = 150f;

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

        // Check distance to player (if assigned)
        if (player != null && Vector3.Distance(position, player.position) < avoidPlayerRadius)
            return false;

        // Check distance to big island (if assigned)
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
    #endregion
}
