using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class IslandManager : NetworkBehaviour
{
    public List<GameObject> islandPrefabs;
    public int numIslands = 10;
    public Vector3 worldMin = new Vector3(0f, 1f, 0f);
    public Vector3 worldMax = new Vector3(1500f, 1f, 1500f);
    public float minDistance = 100f;

    private List<Vector3> usedPositions = new List<Vector3>();

    public override void OnStartServer()
    {
        base.OnStartServer();

        for (int i = 0; i < numIslands; i++)
        {
            GameObject prefab = islandPrefabs[i % islandPrefabs.Count];
            SpawnIsland(prefab);
        }
    }

    [Server]
    public void SpawnIsland(GameObject prefab)
    {
        int attempts = 0;

        while (attempts < 100)
        {
            Vector3 pos = new Vector3(
                Random.Range(worldMin.x, worldMax.x),
                worldMin.y,
                Random.Range(worldMin.z, worldMax.z)
            );

            if (IsPositionAvailable(pos))
            {
                GameObject islandObj = Instantiate(prefab, pos, Quaternion.identity);

                NetworkServer.Spawn(islandObj); // <-- Multiplayer-safe spawn

                Island islandScript = islandObj.GetComponent<Island>();
                if (islandScript != null)
                {
                    islandScript.manager = this;
                    islandScript.islandPrefab = prefab;
                }

                usedPositions.Add(pos);
                return;
            }

            attempts++;
        }

        Debug.LogWarning("Couldn't find valid spawn position for island after 100 attempts.");
    }

    [Server]
    public void NotifyIslandDespawn(Island island)
    {
        Vector3 oldPos = island.transform.position;
        usedPositions.Remove(oldPos);
    }

    private bool IsPositionAvailable(Vector3 pos)
    {
        foreach (Vector3 used in usedPositions)
        {
            if (Vector3.Distance(pos, used) < minDistance)
                return false;
        }
        return true;
    }
}
