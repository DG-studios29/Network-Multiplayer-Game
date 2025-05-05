using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class CannonCollision : NetworkBehaviour
{
    public GameObject parentObj;

    public float cannonLifeSpan;
    public float cannonDamage;

    public float gravityScale;
    private float globalGravity;
    private Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        globalGravity = -9.81f;
       
    }

    // Update is called once per frame
    void Update()
    {
        cannonLifeSpan -= Time.deltaTime;
        if(cannonLifeSpan <= 0)
        {
           NetworkServer.Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        Vector3 gravity = globalGravity * Vector3.up * gravityScale;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should process collisions to avoid desync
        //if (!isServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
          
            PlayerHealthUI playerHealth = collision.gameObject.GetComponent<PlayerHealthUI>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)cannonDamage);
            }
        }

        // Destroy the cannonball on all clients
        NetworkServer.Destroy(gameObject);
    }


}
