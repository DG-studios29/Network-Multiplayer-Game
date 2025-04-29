using UnityEngine;
using UnityEngine.InputSystem;


public enum ActivePanel { PAUSE, CANNON };

public class PlayerInputTemp : MonoBehaviour
{

    [Header("Menu and Button Stuff")]
    public GameObject cannonMenu;
    public GameObject playerHUD;
    public GameObject pauseMenu;


    private InputSystem_Actions pInputAction;

    private PlayerInput playerInput;

    

    public ActivePanel activePanel;
    private HandleEvent handleEvent;


    private CannonHolder cannonHolder;
    private CannonLinq cannonLinq;



    private void Awake()
    {

        pInputAction = new InputSystem_Actions();

        playerInput = GetComponent<PlayerInput>();
       
        

        handleEvent = GameObject.FindAnyObjectByType<HandleEvent>();
        cannonHolder = GameObject.FindAnyObjectByType<CannonHolder>();
        cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activePanel = ActivePanel.CANNON;

        cannonMenu.SetActive(false);
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchToUI()
    {
        //pInputAction.UI.Enable();
        //pInputAction.Player.Disable();


        playerInput.SwitchCurrentActionMap("UI");



    }

    public void SwitchToPlayer()
    {
        //pInputAction.Player.Enable();
        //pInputAction.UI.Disable();

        playerInput.SwitchCurrentActionMap("Player");
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
                //activePanel = ActivePanel.PAUSE;

                handleEvent.ChangeFirstSelected(activePanel);
                //switch maps to navigation
            }
            else
            {
                cannonMenu.SetActive(true);
                playerHUD.SetActive(false);
                activePanel = ActivePanel.CANNON;
                SwitchToUI();
                

                handleEvent.ChangeFirstSelected(activePanel);
                //switch maps to navigation
            }
        }
    }






  
    public void AttackTest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           
            cannonHolder.FireLoadedCannon();

            //cannonLinq.FireCannonChain();
        }
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

                activePanel = ActivePanel.PAUSE;
                //.ChangeFirstSelected(activePanel);
            }
            else
            {
                pauseMenu.SetActive(false);
                playerHUD.SetActive(true);
            }
        }
    }


    public void LoadCannon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cannonHolder.AddToLoadedList();
        }
    }


    public void ClearLoadedCannon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cannonHolder.RemoveFromLoadedList();
        }
    }


    public void ClearAllCannons()
    {
        cannonHolder.ClearLoadedList();
    }


    public void ChangeCannonType(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cannonLinq.ChangeCannonType();
        }
    }
  
}
