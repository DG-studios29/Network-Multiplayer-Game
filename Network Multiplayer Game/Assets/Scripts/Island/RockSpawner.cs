using UnityEngine;
using System.Collections.Generic;

public class RockSpawner : MonoBehaviour
{
    public GameObject[] rockPrefabs;
    public int numberOfRocks = 100;
    public Vector3 spawnAreaSize = new Vector3(1500, 1, 1500);
    public Vector3 spawnCenter = Vector3.zero; 
    public float minDistanceBetweenRocks = 20f;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnRocks();
    }

    void SpawnRocks()
    {
        int rocksSpawned = 0;
        int attempts = 0;

        while (rocksSpawned < numberOfRocks && attempts < numberOfRocks * 10)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(spawnCenter.x - spawnAreaSize.x / 2f, spawnCenter.x + spawnAreaSize.x / 2f),
                spawnCenter.y,
                Random.Range(spawnCenter.z - spawnAreaSize.z / 2f, spawnCenter.z + spawnAreaSize.z / 2f)
            );

            if (SpawnManager.Instance.IsPositionAvailable(randomPos)) 
            {
                GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
                Instantiate(prefab, randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                SpawnManager.Instance.RegisterPosition(randomPos); 
                rocksSpawned++;
            }

            attempts++;
        }
    }

    bool IsPositionFarEnough(Vector3 pos)
    {
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(pos, existingPos) < minDistanceBetweenRocks)
                return false;
        }
        return true;
    }
}
