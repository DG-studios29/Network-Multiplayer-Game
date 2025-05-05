using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LootRadiusVisualizer : MonoBehaviour
{
    public float radius = 100f;
    public int segments = 64;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.5f;

    private LineRenderer line;
    private bool isPlayerInside = false;
    private float pulseTime = 0f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;  // Local space to the island object
        line.loop = true;
        line.positionCount = segments + 1;  // One extra point to close the circle
    }

    void Update()
    {
        // Only show and pulse if the player is inside the radius
        if (isPlayerInside)
        {
            line.enabled = true;  // Show the line renderer

            // Animate the pulsing effect
            pulseTime += Time.deltaTime * pulseSpeed;
            float pulseValue = Mathf.Sin(pulseTime) * pulseAmount + 1f;  // Sin for pulsing
            line.startWidth = pulseValue;  // Apply pulse to line width
            line.endWidth = pulseValue;    // Apply pulse to line width

            // Update the circle points
            DrawCircle();
        }
        else
        {
            line.enabled = false;  // Hide the line renderer when no player is inside
        }
    }

    void DrawCircle()
    {
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.1f, z)); // Slightly above the ground
        }
    }

    // This method should be called when a player enters or exits the radius
    public void SetPlayerInside(bool inside)
    {
        isPlayerInside = inside;
    }
}
