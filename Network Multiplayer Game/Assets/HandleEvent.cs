using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleEvent : MonoBehaviour
{

    private EventSystem eSystem;
    public GameObject pauseBtn;
    public GameObject cannonBtn;
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


}
