using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FloatingMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 10f;
    public float turnTorque = 5f;

    [Header("Input Actions")]
    public InputActionAsset inputActions; 
    private InputAction moveAction;
    private InputAction turnAction;
    private Island island;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        island = GetComponent<Island>();
        // Set up input actions
        var gameplayMap = inputActions.FindActionMap("Player");
        moveAction = gameplayMap.FindAction("Sail");
        turnAction = gameplayMap.FindAction("Turn");
        
    }

    void OnEnable()
    {
        moveAction?.Enable();
        turnAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        turnAction?.Disable();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
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


    void FixedUpdate()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float turnInput = turnAction.ReadValue<float>();

        // Forward/backward movement
        Vector3 forward = transform.forward * moveInput.y;
        if (forward.sqrMagnitude > 0.01f)
        {
            rb.AddForce(forward * moveForce, ForceMode.Force);
        }

        // Turning (left/right)
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            rb.AddTorque(Vector3.up * turnInput * turnTorque, ForceMode.Force);
        }
    }
}
