using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;


[RequireComponent(typeof(NetworkRigidbodyReliable))]
public class FloatingMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 10f;
    public float turnTorque = 5f;

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction turnAction;

    [Header("Stabilization Settings")]
    public float uprightStabilizationStrength = 10f;
    public float uprightStabilizationDamping = 1f;


    private Island island;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        island = GetComponent<Island>();

        var gameplayMap = inputActions.FindActionMap("Player");
        moveAction = gameplayMap.FindAction("Move");
        turnAction = gameplayMap.FindAction("Turn");
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (turnAction != null) turnAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (turnAction != null) turnAction.Disable();
    }

    private void Update()
    {
        // Only run on local client
        if (!isLocalPlayer) return;

        //Debug input to damage nearby islands
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            Collider[] nearby = Physics.OverlapSphere(transform.position, 50f);
            foreach (Collider col in nearby)
            {
                Island island = col.GetComponent<Island>();
                if (island != null)
                {
                    Debug.Log("DEBUG: Instantly destroying island: " + island.name);
                    island.TakeDamage(100f);
                }
            }
        }
    }

    private void StabilizeUpright()
    {
        // Desired up direction (world up)
        Vector3 up = transform.up;
        Vector3 desiredUp = Vector3.up;

        // Calculate the torque required to align the up vector
        Vector3 torqueVector = Vector3.Cross(up, desiredUp);

        // Apply torque to correct tilt
        rb.AddTorque(torqueVector * uprightStabilizationStrength - rb.angularVelocity * uprightStabilizationDamping);
    }


    void FixedUpdate()
    {
        // Only run physics input for local player
        if (!isLocalPlayer) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float turnInput = turnAction.ReadValue<float>();

        CmdMoveIsland(moveInput, turnInput);
    }

    [Command]
    void CmdMoveIsland(Vector2 moveInput, float turnInput)
    {
        Vector3 forward = transform.forward * moveInput.y;
        if (forward.sqrMagnitude > 0.01f)
        {
            rb.AddForce(forward * moveForce, ForceMode.Force);
        }

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            rb.AddTorque(Vector3.up * turnInput * turnTorque, ForceMode.Force);
        }

        StabilizeUpright(); // Run this here too if you want stabilization to also be server-simulated
    }

}
