using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

//https://www.youtube.com/watch?v=8VVgIjWBXks
[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isLocalPlayer)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Convert local movement to world space
        move = transform.TransformDirection(move);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;

        // Move the character
        controller.Move(move * moveSpeed * Time.deltaTime + velocity * Time.deltaTime);
    }
}
