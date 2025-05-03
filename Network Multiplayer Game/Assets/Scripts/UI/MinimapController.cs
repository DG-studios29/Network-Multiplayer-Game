using UnityEngine;
using Mirror;

public class MinimapController : NetworkBehaviour
{
    [SerializeField] private RectTransform playerIcon; 
    [SerializeField] private RectTransform minimap;    
    [SerializeField] private Camera playerCamera;      

    private Transform playerTransform;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        
        playerTransform = transform;

      
        minimap.gameObject.SetActive(true);

       
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer || playerTransform == null || playerCamera == null) return;

        
        playerIcon.rotation = Quaternion.Euler(0, 0, -playerCamera.transform.eulerAngles.y);
    }
}