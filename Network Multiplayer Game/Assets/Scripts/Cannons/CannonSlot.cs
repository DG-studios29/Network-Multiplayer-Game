using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class CannonSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    [SerializeField] private int index; //identify our cannon
    [SerializeField] private int loadNo; //number it will be fired out in our sequence

    [SerializeField]private TMP_Text loadText;
    [SerializeField] private Button thisCannonButton;
    private bool btnSelected;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCannonButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IndexSet(int idx)
    {
        index = idx;
    
    }

    public int GetSlotIndex()
    {
        return index;
    }

    public int GetSlotLoad()
    {
        return loadNo;
    }

    public void ResetCannon()
    {
        loadNo = 0;
        loadText.text = "-";
    }

    public void SetCannon(int lText)
    {

        loadNo = lText;
        loadText.text = lText.ToString();
    }

   public void CannonDisable()
    {
        thisCannonButton.enabled = false;
    }

    public void CannonEnable()
    {
        thisCannonButton.enabled = true;
    }

    public void SwitchSelection()
    {
        Debug.Log("Selected btn Cannon " + index.ToString());
        btnSelected = true;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Selected btn Cannon " + index.ToString());
        btnSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("Deselected btn Cannon " + index.ToString());
        btnSelected = false;
    }

    public bool CheckBtnSelection()
    {
        return btnSelected;
    }


}
