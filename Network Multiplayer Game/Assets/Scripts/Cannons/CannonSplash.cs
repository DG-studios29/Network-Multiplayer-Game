using UnityEngine;
using Mirror;

public class CannonSplash : NetworkBehaviour
{
    public GameObject splashFX;
    public GameObject randomSplashFX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cannonball")
        {

            Vector3 location = other.gameObject.transform.position;
            GameObject splash = Instantiate(splashFX, location, Quaternion.identity);
            Destroy(splash, 1f);
        }

       /* GameObject random = Instantiate(splashFX, other.transform.position, Quaternion.identity);
        Destroy(random, 1f);*/

    }

}
