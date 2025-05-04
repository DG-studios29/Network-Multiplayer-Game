using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class Cannonball : NetworkBehaviour
{
    public int damage = 2;

    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should process collisions to avoid desync
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthUI playerHealth = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Destroy the cannonball on all clients
        NetworkServer.Destroy(gameObject);
    }
}
