using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform target; // The player the minimap camera follows
    public Vector3 offset = new Vector3(0, 50f, 0); // Offset of the minimap camera from the player

    void LateUpdate()
    {
        if (target)
        {
            // Update the minimap camera's position
            transform.position = target.position + offset;

            // Keep the minimap camera's rotation fixed (top-down view)
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}