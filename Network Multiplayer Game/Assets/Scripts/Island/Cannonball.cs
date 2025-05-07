using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Cannonball : NetworkBehaviour
{
    public int damage = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthUI health = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (health != null) health.TakeDamage(damage);
        }

        NetworkServer.Destroy(gameObject);
    }
}