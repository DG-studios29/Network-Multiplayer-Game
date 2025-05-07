using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class RockSpawner : NetworkBehaviour
{
    public GameObject[] rockPrefabs;
    public int numberOfRocks = 100;
    public Vector3 spawnAreaSize = new Vector3(1500, 1, 1500);
    public Vector3 spawnCenter = Vector3.zero;
    public float minDistanceBetweenRocks = 20f;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    public override void OnStartServer() 
    {
        base.OnStartServer();
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
                GameObject rock = Instantiate(prefab, randomPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

                NetworkServer.Spawn(rock); // ?? Sync with all clients
                SpawnManager.Instance.RegisterPosition(randomPos);

                rocksSpawned++;
            }

            attempts++;
        }
    }
}
