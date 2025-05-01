using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MinimapInitializer : NetworkBehaviour
{
    public GameObject minimapCamPrefab;   
    public GameObject minimapUIPrefab;     
    private RawImage minimapUI;            
    private GameObject minimapCam;        

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        
        minimapCam = Instantiate(minimapCamPrefab);

       
        if (minimapUIPrefab != null)
        {
            minimapUI = Instantiate(minimapUIPrefab).GetComponent<RawImage>();
        }

       
        MinimapCameraFollow follow = minimapCam.GetComponent<MinimapCameraFollow>();
        follow.target = transform;

       
        if (minimapUI != null)
        {
            follow.minimapUI = minimapUI;
        }

   
        minimapCam.GetComponent<Camera>().enabled = true;
        minimapUI.gameObject.SetActive(true);
    }

    public override void OnStopLocalPlayer()
    {
       
        if (minimapCam != null)
        {
            Destroy(minimapCam);
        }

        if (minimapUI != null)
        {
            Destroy(minimapUI.gameObject);
        }
    }
}