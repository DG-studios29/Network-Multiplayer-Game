using UnityEngine;

public class RockDamage : MonoBehaviour
{
    public int damageAmount = 10; // Damage to player on hit

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Rock");
            //PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            //if (playerHealth != null)
            //{
            //    playerHealth.TakeDamage(damageAmount);
            //}
        }
    }
}
