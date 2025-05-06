using UnityEngine;
using Mirror;

public class RockDamage : NetworkBehaviour
{
    public int damageAmount = 1;

    private void OnCollisionEnter(Collision collision)
    {
       // if (!isServer) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Rock");

            PlayerHealthUI playerHealth = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); 
            }
        }
    }
}
