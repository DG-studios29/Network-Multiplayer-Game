using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;


public enum ActivePanel {  CANNON , PRESETS};

public class PlayerInputTemp : NetworkBehaviour
{

    [Header("Menu and Button Stuff")]
    public GameObject cannonMenu;
    public GameObject playerHUD;
    public GameObject pauseMenu;
    public GameObject mainMap;
    public GameObject presetsPanel;
    public GameObject loadingPanel;

    private InputSystem_Actions pInputAction;

    private PlayerInput playerInput;


    [Header("References")]
    public ActivePanel activePanel;
    [SerializeField]private HandleEvent handleEvent;


    [SerializeField]private CannonHolder cannonHolder;
    [SerializeField] private CannonLinq cannonLinq;

    [SerializeField] private CanvasLoadHUD canvasLoadHUD;
    [SerializeField] private PresetInput presetInput;
    [SerializeField]private bool isPresetLoading;


    private NetworkIdentity netID;

    public NetworkIdentity NetID;
    [SerializeField]private string captainName;
    [SerializeField]private int numJoined;

    public string CaptainName => CaptainName;
    public int NumJoined => numJoined;


    [SerializeField] private NetTempFinder netTempFinder;


    private void Awake()
    {
        

        pInputAction = new InputSystem_Actions();

        playerInput = GetComponent<PlayerInput>();

        if (netTempFinder == null)
        {

        }
        else
        {

            handleEvent = GameObject.FindAnyObjectByType<HandleEvent>();
            cannonHolder = GameObject.FindAnyObjectByType<CannonHolder>();
            cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
            canvasLoadHUD = GameObject.FindAnyObjectByType<CanvasLoadHUD>();
            presetInput = GameObject.FindAnyObjectByType<PresetInput>();
        }

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (!isLocalPlayer && playerHUD != null && cannonMenu != null)
        {
            playerHUD.SetActive(false);
            cannonMenu.SetActive(false);
            return;
        }

        activePanel = ActivePanel.CANNON;

        cannonMenu.SetActive(false);
        presetsPanel.SetActive(false);
      
    }

    // Update is called once per frame
    
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

    void MultiplayerBlock()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }


    void SwitchToUI()
    {
        //pInputAction.UI.Enable();
        //pInputAction.Player.Disable();
        MultiplayerBlock();

        playerInput.SwitchCurrentActionMap("UI");



    }

    public void SwitchToPlayer()
    {
        //pInputAction.Player.Enable();
        //pInputAction.UI.Disable();

        MultiplayerBlock();

        playerInput.SwitchCurrentActionMap("Player");
    }


    [ClientCallback] 
    public void ToggleCannonMenu(InputAction.CallbackContext context)
    {
     
        if (context.performed)
        {
            MultiplayerBlock();

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




    [TargetRpc]
    public void TargetShowWelcomeMessage(NetworkConnection target, string message)
    {
       /*  if (target.identity != this.NetID)
         {
            //uiManager.ShowMessage(message);
            return;
         }
*/
        //StartCoroutine(ShowMessage(message));
        cannonLinq.MsgHUD(message);

    }


    [TargetRpc]
    public void SetPlayerName(NetworkConnection target, string name, int count)
    {
        /* if (uiManager != null)
         {
             uiManager.ShowMessage(message);
         }*/
        /*     if (target.identity != this.NetID)
             {
                 //uiManager.ShowMessage(message);
                 return;
             }*/


        //StartCoroutine(ShowMessage(message));
        captainName = name;
        numJoined = count;
        cannonLinq.NameHUD($"Player {count.ToString()}");
        

    }


    public void AttackTest(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            MultiplayerBlock();

            cannonHolder.FireLoadedCannon();
            

            //cannonLinq.FireCannonChain();
        }
    }




/*    public void YouCantPauseOnline(InputAction.CallbackContext context)
    {
       

        if (context.performed)
        {
            MultiplayerBlock();

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
*/



    public void OpenPresetsMenu(InputAction.CallbackContext context)
    {
      

        if (context.performed)
        {
            MultiplayerBlock();

            if (loadingPanel.activeSelf)
            {


                presetsPanel.SetActive(true);
                loadingPanel.SetActive(false);
                

                activePanel = ActivePanel.PRESETS;
               
                handleEvent.ChangeFirstSelected(ActivePanel.PRESETS);

                canvasLoadHUD.ShowOpenedPanel(activePanel);
            }

        }
    }


    public void OpenLoadoutMenu(InputAction.CallbackContext context)
    {
       

        if (context.performed)
        {
            MultiplayerBlock();

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
            MultiplayerBlock();

            cannonHolder.AddToLoadedList();
        }
    }


    public void ClearLoadedCannon(InputAction.CallbackContext context)
    {
      

        if (context.performed)
        {
            MultiplayerBlock();

            cannonHolder.RemoveFromLoadedList();
        }
    }


    public void ClearAllCannons()
    {
        MultiplayerBlock();

        cannonHolder.ClearLoadedList();
    }





    public void ChangeCannonType(InputAction.CallbackContext context)
    {

        if (context.performed)
        {

            MultiplayerBlock();
            cannonLinq.ChangeCannonType();
        }
    }


    public void ActivePanelSet(ActivePanel active)
    {
        MultiplayerBlock();

        activePanel = active;

        handleEvent.ChangeFirstSelected(active);
    }
  

    public void PresetHold(InputAction.CallbackContext context)
    {
    

        if (context.performed)
        {
            MultiplayerBlock();

            presetInput.HoldActivated();
            isPresetLoading = true;
        }
        if (context.canceled)
        {
            MultiplayerBlock();

            presetInput.HoldDropped();
            isPresetLoading = false;
        }
    }

    public void AutoLoadPreset(InputAction.CallbackContext context)
    {

       
        if (context.performed)
        {
            MultiplayerBlock();

            if (isPresetLoading == true)
            {
                //presetInput.LoadUpPreset();
                cannonHolder.UsePresetToLoad();
            }
        }
    }

    public void mainMapToogle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(mainMap.activeSelf)
            {
                mainMap.SetActive(false);
            }
            else
            {
                mainMap.SetActive(true);
            }
           
        }
      
    }

}
