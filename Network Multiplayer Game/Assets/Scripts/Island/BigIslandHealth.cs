using Mirror;
using UnityEngine;

public class BigIslandHealth : NetworkBehaviour
{
    public int health = 500;
    public GameObject lootZonePrefab;

    [Server]
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            RpcOnDestroyed();
            Object.FindAnyObjectByType<GameTimer>()?.EndGameEarly("Big Island destroyed!");
        }
    }

    [ClientRpc]
    void RpcOnDestroyed()
    {
        if (isServer)
        {
            Instantiate(lootZonePrefab, transform.position, Quaternion.identity);
        }
    }
}
