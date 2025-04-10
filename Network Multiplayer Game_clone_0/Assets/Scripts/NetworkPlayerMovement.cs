using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
//https://www.youtube.com/watch?v=8VVgIjWBXks

public class NetworkPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f; // Player movement speed
    public float jumpForce = 5f; // Jump force
    private Rigidbody rb;

    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This method will be called by the input action
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isLocalPlayer) // Ensure the input is only handled for the local player
        {
            moveInput = context.ReadValue<Vector2>(); // Get movement input (x, y)
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer) // Only execute movement logic for the local player
        {
            MovePlayer();
        }
    }

    // Move the player based on input
    private void MovePlayer()
    {
        // Convert input to movement in world space
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
