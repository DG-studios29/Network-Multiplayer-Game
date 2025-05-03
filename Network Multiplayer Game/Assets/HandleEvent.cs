using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleEvent : MonoBehaviour
{

    private EventSystem eSystem;
    public GameObject pauseBtn;
    public GameObject cannonBtn;
    public GameObject presetsBtn;
 
    private PlayerInputTemp pInpTemp;



    private void Awake()
    {
        eSystem = GetComponent<EventSystem>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if(eSystem.firstSelectedGameObject.)
    }

    // Update is called once per frame
    void Update()
    {
        //if()
    }

    //Just remembered you can't pause online
    public void ChangeFirstSelected(ActivePanel activePanel) 
    { 
        if(activePanel == ActivePanel.PAUSE)
        {
            PauseActive();
        }
        else if(activePanel == ActivePanel.CANNON)
        {
            CannonActive();
        }
        else if(activePanel == ActivePanel.PRESETS)
        {
            PresetsActive();
        }
    }



    void PauseActive()
    {
        if(eSystem.firstSelectedGameObject != null)
        {
            eSystem.firstSelectedGameObject = pauseBtn;
            Button button = eSystem.firstSelectedGameObject.GetComponent<Button>();
            button.Select();
        }
    }

    void CannonActive()
    {

        if (eSystem.firstSelectedGameObject != null)
        {
            eSystem.firstSelectedGameObject = cannonBtn;
            Button button = eSystem.firstSelectedGameObject.GetComponent<Button>();
            button.Select();
            button.GetComponent<CannonSlot>().SwitchSelection();
        }
    }

    void PresetsActive()
    {
        if (eSystem.firstSelectedGameObject != null)
        {
            eSystem.firstSelectedGameObject = presetsBtn;
            Button button = eSystem.firstSelectedGameObject.GetComponent<Button>();
            button.Select();

            PresetsBtn preset = button.GetComponent<PresetsBtn>();
            preset.SwitchSelection();


            if (!preset.GetAllHighlightStatus())
            {
                //if no presets are selected, select the first
                preset.HighlightActive();
            }

        }
    }

}
