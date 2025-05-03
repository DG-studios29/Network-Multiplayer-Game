using UnityEngine;
using Mirror;

public class MinimapPlayerIcon : NetworkBehaviour
{
    [Header("Minimap Settings")]
    public RectTransform minimapImage;
    public RectTransform playerIcon; 
    public Camera minimapCamera; 
    public float minimapScale = 1f; 

    private void Start()
    {
        
        if (!isLocalPlayer)
        {
            enabled = false; 
            return;
        }

       
        if (minimapImage == null || playerIcon == null || minimapCamera == null)
        {
            Debug.LogError("Minimap image, player icon, or minimap camera is not assigned!");
            return;
        }

        Debug.Log("MinimapPlayerIcon initialized for local player.");
    }

    private void Update()
    {
        if (!isLocalPlayer || playerIcon == null || minimapImage == null || minimapCamera == null) return;

       
        Vector2 minimapPosition = WorldToMinimapPosition(transform.position);

        
        playerIcon.localPosition = minimapPosition;
        playerIcon.localRotation = Quaternion.Euler(0, 0, -transform.eulerAngles.y); 
    }

    private Vector2 WorldToMinimapPosition(Vector3 worldPosition)
    {
       
        Vector3 viewportPosition = minimapCamera.WorldToViewportPoint(worldPosition);

        
        float x = (viewportPosition.x - 0.5f) * minimapImage.rect.width * minimapScale;
        float y = (viewportPosition.y - 0.5f) * minimapImage.rect.height * minimapScale;

        return new Vector2(x, y);
    }
}