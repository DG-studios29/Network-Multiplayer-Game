using UnityEngine;
using Mirror;

[RequireComponent(typeof(LineRenderer))]
public class LootRadiusVisualizer : NetworkBehaviour
{
    public float radius = 80f;
    public int segments = 64;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.5f;

    [SyncVar(hook = nameof(OnRadiusVisibilityChanged))]
    private bool showRadius;

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

        line.material = new Material(Shader.Find("Unlit/Color"));
        line.material.color = Color.cyan;
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
        if (isServer)
        {
            showRadius = visible;
        }
    }

    private void OnRadiusVisibilityChanged(bool oldVal, bool newVal)
    {
        isVisible = newVal;
        line.enabled = newVal;

        if (newVal)
        {
            DrawCircle();
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
            line.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }
}
