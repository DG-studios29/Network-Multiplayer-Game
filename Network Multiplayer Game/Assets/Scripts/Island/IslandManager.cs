using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//https://stackoverflow.com/questions/70368042/how-to-spawn-objects-in-unity3d-with-a-minimum-distance-between#:~:text=%2F%2F%20It%20is%20cheaper%20to,minimumDistance
//https://stackoverflow.com/questions/70368042/how-to-spawn-objects-in-unity3d-with-a-minimum-distance-between#:~:text=,to%20an%20already%20existing%20one

public class IslandManager : MonoBehaviour
{
    public List<GameObject> islandPrefabs;  
    public int numIslands = 10;
    public Vector3 worldMin = new Vector3(0f, 1f, 0f);
    public Vector3 worldMax = new Vector3(1500f, 1f, 1500f);
    public float minDistance = 100f;
   

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        
        for (int i = 0; i < numIslands; i++)
        {
            // Choose a random prefab or use i to pick each unique type once
            GameObject prefab = islandPrefabs[i % islandPrefabs.Count];
            SpawnIsland(prefab);
        }
    }

    // Spawns one island of given prefab at a random valid location.
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

            if (SpawnManager.Instance.IsPositionAvailable(pos))
            {
                GameObject islandObj = Instantiate(prefab, pos, Quaternion.identity);
                SpawnManager.Instance.RegisterPosition(pos);

                Island islandScript = islandObj.GetComponent<Island>();
                if (islandScript != null)
                {
                    islandScript.manager = this;
                    islandScript.islandPrefab = prefab;
                }

                return; // success, leave the function
            }

            attempts++;
        }

        Debug.LogWarning("Couldn't find valid spawn position for island after 100 attempts.");
    }




    public void NotifyIslandDespawn(Island island)
    {
        
        Vector3 oldPos = island.transform.position;
        usedPositions.Remove(oldPos);
    }
}
