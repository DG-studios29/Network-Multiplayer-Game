using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadText : MonoBehaviour
{
    [SerializeField] private int displayIndex;
    [SerializeField] private CannonSlot linkedSlot;
    [SerializeField] private TMP_Text displayText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayText = GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignIndex(int idx)
    {
        displayIndex = idx;
    }


    public void AssignSlot(CannonSlot cSlot)
    {
        linkedSlot = cSlot;
    }

    public void CheckLoad()
    {
        if(linkedSlot != null)
        {
            if (linkedSlot.GetSlotLoad() != 0)
            {
               DisplayDetails();
            }
            else
            {
                ClearDetails();
            }
        }
    }


    private void DisplayDetails()
    {
        displayText.text = linkedSlot.GetSlotLoad().ToString();
    }

    private void ClearDetails()
    {
        displayText.text = "#";
    }

}
