using UnityEngine;
using Mirror;

public class MinimapInitializer : NetworkBehaviour
{
    public GameObject minimapCamPrefab;   // Prefab for the minimap camera
    public RectTransform compassImage;   // The compass image (static)
    public RectTransform playerIcon;     // The player icon (rotates)
    public RectTransform minimapImage;   // The minimap image (center is the reference point)
    public float minimapScale = 1f;      // Scale factor for minimap coordinates

    private GameObject minimapCam;       // Instance of the minimap camera

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Instantiate the minimap camera for the local player
        if (minimapCamPrefab != null)
        {
            minimapCam = Instantiate(minimapCamPrefab);
            MinimapCameraFollow follow = minimapCam.GetComponent<MinimapCameraFollow>();
            if (follow != null)
            {
                follow.target = transform; // Set the player as the target
            }
        }

        // Ensure the compass and player icon are assigned
        if (compassImage == null || playerIcon == null || minimapImage == null)
        {
            Debug.LogError("Compass image, player icon, or minimap image is not assigned!");
            return;
        }

        Debug.Log("MinimapInitializer initialized for local player.");
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();

        // Clean up the minimap camera when the player is destroyed
        if (minimapCam != null)
        {
            Destroy(minimapCam);
        }
    }
}