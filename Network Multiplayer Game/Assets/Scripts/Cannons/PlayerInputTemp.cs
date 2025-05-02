using UnityEngine;
using UnityEngine.InputSystem;


public enum ActivePanel { PAUSE, CANNON , PRESETS};

public class PlayerInputTemp : MonoBehaviour
{

    [Header("Menu and Button Stuff")]
    public GameObject cannonMenu;
    public GameObject playerHUD;
    public GameObject pauseMenu;

    public GameObject presetsPanel;
    public GameObject loadingPanel;

    private InputSystem_Actions pInputAction;

    private PlayerInput playerInput;

    

    public ActivePanel activePanel;
    private HandleEvent handleEvent;


    private CannonHolder cannonHolder;
    private CannonLinq cannonLinq;

    private CanvasLoadHUD canvasLoadHUD;
    private PresetInput presetInput;
    private bool isPresetLoading;

    private void Awake()
    {

        pInputAction = new InputSystem_Actions();

        playerInput = GetComponent<PlayerInput>();
       
        

        handleEvent = GameObject.FindAnyObjectByType<HandleEvent>();
        cannonHolder = GameObject.FindAnyObjectByType<CannonHolder>();
        cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
        canvasLoadHUD = GameObject.FindAnyObjectByType<CanvasLoadHUD>();
        presetInput = GameObject.FindAnyObjectByType<PresetInput>();


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activePanel = ActivePanel.CANNON;

        cannonMenu.SetActive(false);
        presetsPanel.SetActive(false);
      
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

                loadingPanel.SetActive(true);
                presetsPanel.SetActive(false);

                SwitchToPlayer();
                //activePanel = ActivePanel.PAUSE;

   
                handleEvent.ChangeFirstSelected(activePanel);
                //switch maps to navigation
            }
            else
            {
                cannonMenu.SetActive(true);
                playerHUD.SetActive(false);

                loadingPanel.SetActive(true);
                presetsPanel.SetActive(false);

                activePanel = ActivePanel.CANNON;
                SwitchToUI();
                

                canvasLoadHUD.ShowOpenedPanel(activePanel);
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




    public void OpenPresetsMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (loadingPanel.activeSelf)
            {
                loadingPanel.SetActive(false);
                presetsPanel.SetActive(true);

                activePanel = ActivePanel.PRESETS;
               
                handleEvent.ChangeFirstSelected(activePanel);

                canvasLoadHUD.ShowOpenedPanel(activePanel);
            }

        }
    }


    public void OpenLoadoutMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (presetsPanel.activeSelf)
            {
                loadingPanel.SetActive(true);
                presetsPanel.SetActive(false);

                activePanel = ActivePanel.CANNON;
               
                handleEvent.ChangeFirstSelected(activePanel);

                canvasLoadHUD.ShowOpenedPanel(activePanel);
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


    public void ActivePanelSet(ActivePanel active)
    {
        activePanel = active;

        handleEvent.ChangeFirstSelected(active);
    }
  

    public void PresetHold(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            presetInput.HoldActivated();
            isPresetLoading = true;
        }
        if (context.canceled)
        {
            presetInput.HoldDropped();
            isPresetLoading = false;
        }
    }

    public void AutoLoadPreset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(isPresetLoading == true)
            {
                //presetInput.LoadUpPreset();
                cannonHolder.UsePresetToLoad();
            }
        }
    }

}
