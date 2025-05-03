using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasLoadHUD : MonoBehaviour
{
    [SerializeField] private Image loadPanelBtn;
    [SerializeField] private Image presetPanelBtn;

    public GameObject presetsPanel;
    public GameObject loadingPanel;

    [SerializeField] PlayerInputTemp playerInputTemp;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       playerInputTemp = GameObject.FindAnyObjectByType<PlayerInputTemp>();
       //ShowOpenedPanel(ActivePanel.CANNON);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowOpenedPanel(ActivePanel active)
    {
        if(active == ActivePanel.CANNON)
        {
            loadPanelBtn.enabled = true;
            presetPanelBtn.enabled = false;
        }
        else if(active == ActivePanel.PRESETS)
        {
            loadPanelBtn.enabled=false;
            presetPanelBtn.enabled=true;
        }
    }


    public void PresetsCheck()
    {
        if (loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(false);
            presetsPanel.SetActive(true);

            ShowOpenedPanel(ActivePanel.PRESETS);
            playerInputTemp.ActivePanelSet(ActivePanel.PRESETS);
           
        }
    }


    public void LoadoutCheck()
    {
        if (presetsPanel.activeSelf)
        {
            loadingPanel.SetActive(true);
            presetsPanel.SetActive(false);

          
            ShowOpenedPanel(ActivePanel.CANNON);
            playerInputTemp.ActivePanelSet(ActivePanel.CANNON);
        }

    }

}
