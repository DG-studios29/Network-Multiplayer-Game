using UnityEngine;
using Mirror;

[RequireComponent(typeof(LineRenderer))]
public class LootRadiusVisualizer : NetworkBehaviour
{
    public float radius = 100f;
    public int segments = 64;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.5f;

    private LineRenderer line;
    private bool isVisible = false;
    private float pulseTime = 0f;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = segments + 1;
        line.enabled = false;
    }

    void Update()
    {
        if (!isVisible) return;

        pulseTime += Time.deltaTime * pulseSpeed;
        float pulseValue = Mathf.Sin(pulseTime) * pulseAmount + 1f;
        line.startWidth = pulseValue;
        line.endWidth = pulseValue;

        DrawCircle();
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        line.enabled = visible;
    }

    void DrawCircle()
    {
        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }
}
