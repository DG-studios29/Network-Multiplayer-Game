using UnityEngine;
using UnityEngine.InputSystem;


public enum ActivePanel { PAUSE, CANNON };

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


    [Header("Menu and Button Stuff")]
    public GameObject cannonMenu;
    public GameObject playerHUD;
    public GameObject pauseMenu;


    private InputSystem_Actions pInputAction;

    

    public ActivePanel activePanel;
    private HandleEvent handleEvent;




    private void Awake()
    {

        pInputAction = new InputSystem_Actions();

        handleEvent = GameObject.FindAnyObjectByType<HandleEvent>();

        rb = GetComponent<Rigidbody>(); // Ensure cube has a Rigidbody component
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activePanel = ActivePanel.PAUSE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void ToggleCannonMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (cannonMenu.activeSelf)
            {
                cannonMenu.SetActive(false);
                playerHUD.SetActive(true);
                SwitchToPlayer();
                activePanel = ActivePanel.PAUSE;

                handleEvent.ChangeFirstSelected(activePanel);
                //switch maps to navigation
            }
            else
            {
                cannonMenu.SetActive(true);
                playerHUD.SetActive(false);
                SwitchToUI();
                activePanel = ActivePanel.CANNON;

                handleEvent.ChangeFirstSelected(activePanel);
                //switch maps to navigation
            }
        }
    }





    void SwitchToUI()
    {
        pInputAction.Player.Disable();
        pInputAction.UI.Enable();
    }

    public void SwitchToPlayer()
    {
        pInputAction.Player.Enable();
        pInputAction.UI.Disable();
    }

  
    public void YouCantPauseOnline(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);
                cannonMenu.SetActive(false);
                playerHUD.SetActive(false);
            }
            else
            {
                pauseMenu.SetActive(false);
                playerHUD.SetActive(true);
            }
        }
    }



  
}
