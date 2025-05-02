using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
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
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(position, occupied) < minDistanceBetweenObjects)
                return false;
        }
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
