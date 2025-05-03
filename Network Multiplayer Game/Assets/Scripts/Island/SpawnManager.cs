using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance;

    public List<Vector3> occupiedPositions = new List<Vector3>();
    public float minDistanceBetweenObjects = 50f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsPositionAvailable(Vector3 position)
    {
        if (!isServer) return false;

        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }
        return true;
    }

    public void RegisterPosition(Vector3 position)
    {
        if (!isServer) return;
        occupiedPositions.Add(position);
    }

    public void UnregisterPosition(Vector3 position)
    {
        if (!isServer) return;
        occupiedPositions.Remove(position);
    }
}
