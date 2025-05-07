using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [Header("Camera References")]
    public Camera myCam;
    public Camera minimap;
    private Camera mainMapCam;

    void Start()
    {
        // Assign main map camera if not already assigned
        if (!mainMapCam)
        {
            GameObject mapCamObj = GameObject.Find("MapCamera");
            if (mapCamObj)
                mainMapCam = mapCamObj.GetComponent<Camera>();
        }

        // Deactivate all cameras if not the local player
        if (!isLocalPlayer)
        {
            DeactivateAllCameras();
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Assign cameras only if null (can be set in inspector or runtime)
        if (!myCam)
            myCam = Camera.main;

        if (!minimap)
            minimap = GameObject.Find("MinimapCamera")?.GetComponent<Camera>();

        if (!mainMapCam)
            mainMapCam = GameObject.Find("MapCamera")?.GetComponent<Camera>();

    }

    private void DeactivateAllCameras()
    {
        if (myCam)
            myCam.gameObject.SetActive(false);

        if (minimap)
            minimap.gameObject.SetActive(false);

        if (mainMapCam)
            mainMapCam.gameObject.SetActive(false);
    }
}
