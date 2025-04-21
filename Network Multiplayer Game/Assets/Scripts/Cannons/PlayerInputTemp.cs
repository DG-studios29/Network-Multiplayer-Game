using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputTemp : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeedAgain = 5f;
    public float jumpForce = 7f;
    private Vector3 cubeDirection;
    private Rigidbody rb;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 10f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Ensure cube has a Rigidbody component
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   


    public void MovePlayerOne(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 playerInput = ctx.ReadValue<Vector2>();
            cubeDirection.x = playerInput.x;
            cubeDirection.z = playerInput.y;
        }
        else if (ctx.canceled)
        {
            cubeDirection = Vector2.zero;
        }
    }

    public void JumpPlayerOne(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Jump");
        }
    }

    public void ShootPlayerOne(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody projRb = projectile.GetComponent<Rigidbody>();
            projRb.linearVelocity = transform.forward * projectileSpeed;

            Destroy(projectile, 3f);
        }
    }

  
}
