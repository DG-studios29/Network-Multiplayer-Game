using UnityEngine;
using System.Collections.Generic;

public class PresetsHolder : MonoBehaviour
{

    private List<PresetsBtn> presets = new List<PresetsBtn>();
    private PresetsBtn[] presetsArray;
    

    [SerializeField] private PresetsBtn preset1;
    [SerializeField] private PresetsBtn preset2;
    [SerializeField] private PresetsBtn preset3;
    [SerializeField] private PresetsBtn preset4;
    [SerializeField] private PresetsBtn preset5;
    [SerializeField] private PresetsBtn preset6;


    private void Awake()
    {
        presetsArray = new PresetsBtn[6];

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //may break on multiplayer
        //presetsArray = GameObject.FindObjectsByType<PresetsBtn>(FindObjectsSortMode.InstanceID);

        presetsArray[0] = preset1;
        presetsArray[1] = preset2;
        presetsArray[2] = preset3;
        presetsArray[3] = preset4;
        presetsArray[4] = preset5;
        presetsArray[5] = preset6;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HighlightOne(PresetsBtn presetSelected)
    {
       
        foreach(PresetsBtn preset in presetsArray)
        {
            if(presetSelected != preset)
            {
                preset.HighlightOff();
            }
        }

       
    }

    public bool CheckHighlight()
    {
        foreach(PresetsBtn preset in presetsArray)
        {
            if (preset.GetHighlightStatus())
            {
                //we already have a higlighted btn
                return true;
            }
        }

        // no buttons are higlighted / no presets selected
        return false;
    }


}
