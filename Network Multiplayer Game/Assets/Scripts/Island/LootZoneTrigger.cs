using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LootZoneTrigger : NetworkBehaviour
{
    public float lootDuration = 5f;

    private readonly SyncDictionary<NetworkIdentity, float> playersInside = new SyncDictionary<NetworkIdentity, float>();

    void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        if (other.CompareTag("Player"))
        {
            NetworkIdentity identity = other.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                playersInside[identity] = 0f;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        if (other.CompareTag("Player"))
        {
            NetworkIdentity identity = other.GetComponent<NetworkIdentity>();
            if (identity != null && playersInside.ContainsKey(identity))
            {
                playersInside.Remove(identity);
            }
        }
    }

    void Update()
    {
        if (!isServer) return;

        foreach (var player in new List<NetworkIdentity>(playersInside.Keys))
        {
            playersInside[player] += Time.deltaTime;

            if (playersInside[player] >= lootDuration)
            {
                TargetGiveLoot(player.connectionToClient);
                playersInside.Remove(player);
            }
        }
    }

    [TargetRpc]
    void TargetGiveLoot(NetworkConnection target)
    {
        Debug.Log("You stayed in the zone. You win the loot!");
        // Add loot to inventory or trigger win state
    }
}
