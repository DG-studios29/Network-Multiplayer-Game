using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public List<Vector3> occupiedPositions = new List<Vector3>();
    public float minDistanceBetweenObjects = 50f;

    [Header("Avoid These")]
    public Transform player;
    public Transform bigIsland;
    public float avoidPlayerRadius = 100f;
    public float avoidBigIslandRadius = 150f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsPositionAvailable(Vector3 position)
    {
        // Check against all previously spawned objects
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }

        // Check distance to player
        if (player != null && Vector3.Distance(position, player.position) < avoidPlayerRadius)
            return false;

        // Check distance to big island
        if (bigIsland != null && Vector3.Distance(position, bigIsland.position) < avoidBigIslandRadius)
            return false;

        return true;
    }

    public void RegisterPosition(Vector3 position)
    {
        occupiedPositions.Add(position);
    }

    public void UnregisterPosition(Vector3 position)
    {
        occupiedPositions.Remove(position);
    }
}
