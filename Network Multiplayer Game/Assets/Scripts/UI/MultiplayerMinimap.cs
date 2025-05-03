using UnityEngine;
using Mirror;

public class MultiplayerMinimap : NetworkBehaviour
{
    [Header("Minimap Settings")]
    public Camera minimapCamera; 
    public RectTransform compassImage; 
    public Vector3 cameraOffset = new Vector3(0, 20, 0); 
    private void Start()
    {
        if (!isLocalPlayer) return;

       
        if (minimapCamera == null)
        {
            minimapCamera = GameObject.FindWithTag("MinimapCamera").GetComponent<Camera>();
        }

        if (minimapCamera == null)
        {
            Debug.LogError("Minimap camera not found! Ensure the camera has the 'MinimapCamera' tag.");
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer || minimapCamera == null) return;

        
        minimapCamera.transform.position = transform.position + cameraOffset;

      
        if (compassImage != null)
        {
            compassImage.localRotation = Quaternion.Euler(0, 0, -transform.eulerAngles.y);
        }
    }
}