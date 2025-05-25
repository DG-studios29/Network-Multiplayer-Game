using Mirror;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransformReliable))]
public class Cannonball : NetworkBehaviour
{
    public int damage = 2;
    public GameObject impactFX;

    private void Start()
    {
      

        if (isClient)
        {
            GetComponent<Renderer>().material.color = Color.red; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthUI health = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (health == null)
                health = collision.gameObject.GetComponentInChildren<PlayerHealthUI>();

            if (health != null)
            {

                health.TakeDamage(damage);
               
            }
            else
            {
                Debug.LogWarning("PlayerHealthUI not found on hit player object.");
            }
        }
        GameObject hit = Instantiate(impactFX, this.transform.position, Quaternion.identity);
        NetworkServer.Destroy(gameObject);
    }
}