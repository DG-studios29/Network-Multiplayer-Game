using UnityEngine;
using UnityEngine.EventSystems;
public class SelectableBtn : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected private bool btnSelected;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SwitchSelection()
    {
        //Debug.Log("Selected btn Cannon " + index.ToString());
        btnSelected = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("Selected btn Cannon " + index.ToString());
        btnSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Debug.Log("Deselected btn Cannon " + index.ToString());
        btnSelected = false;
    }




}
