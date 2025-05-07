using UnityEngine;
using UnityEngine.UI;


public class PresetsBtn : SelectableBtn
{
    [SerializeField] private Image backgroundColour;
    [SerializeField]private PresetsHolder presetHolder;
    private bool isHighlighted = false;

    private LoadPresetLoader presetLoader;


    private void Awake()
    {
        presetHolder = GetComponentInParent<PresetsHolder>();

        presetLoader = GetComponentInChildren<LoadPresetLoader>();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        //not necessary
        HighlightOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void HighlightActive()
    {
        if (btnSelected)
        {
            backgroundColour.enabled = true;
            isHighlighted = true;
            
        }


        if(presetHolder == null)
        {
            presetHolder = GetComponentInParent<PresetsHolder>();
        }
       


        //turn off every other, except this button
        presetHolder.HighlightOne(this);
    }

    public void HighlightOff()
    {
        backgroundColour.enabled=false;
        isHighlighted = false;
        
    }

    public bool GetHighlightStatus()
    {


        if (presetHolder == null)
        {
            presetHolder = GetComponentInParent<PresetsHolder>();
        }


        return isHighlighted;
    }


    public bool GetAllHighlightStatus()
    {
        if(presetHolder == null)
        {
            presetHolder = GetComponentInParent<PresetsHolder>();
        }

        return presetHolder.CheckHighlight();
    }


    public LoadPresetData FindPresetData()
    {

        return presetLoader.LoadPresetData;
    }

}
