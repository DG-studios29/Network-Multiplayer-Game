using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class CannonSlot : SelectableBtn
{

    [SerializeField] private int index; //identify our cannon
    [SerializeField] private int loadNo; //number it will be fired out in our sequence
    [SerializeField] private int multiCount;
    [SerializeField] private int firstMatch;

    [SerializeField] private TMP_Text indexText;
    [SerializeField]private TMP_Text loadText;
    [SerializeField] private TMP_Text multiText;
    [SerializeField] private Button thisCannonButton;
    //private bool btnSelected;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCannonButton = GetComponent<Button>();
        multiText.text = "";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IndexSet(int idx)
    {
        index = idx;
        indexText.text = "Cannon #" + index.ToString();
    
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
        multiText.text = "";
        multiCount = 0;
    }

    public void SetCannon(int lText)
    {

        loadNo = lText;
        loadText.text = lText.ToString();
    }

    public int GetMultiCount()
    {
        return multiCount;
    }


    public void TakeMulti()
    {
        multiCount--;
        SetMultiCount(multiCount);
    }

    public void SetMultiCount(int counts)
    {
        multiCount = counts;
        if(multiCount <= 1)
        {
            multiText.text = "";
        }
        else
        {
            multiText.text = "x" + multiCount.ToString();
        }
    }

    public void FirstMatchMarked(int idx)
    {
        firstMatch = idx;
    }

    public bool AlreadySetMatch(int idx)
    {
        if(firstMatch != 0 && firstMatch != idx)
        {
            return false;
        }
        return true;
    }


   public void CannonDisable()
    {
        thisCannonButton.enabled = false;
    }

    public void CannonEnable()
    {
        thisCannonButton.enabled = true;
    }


    public bool CheckBtnSelection()
    {
        return btnSelected;
    }


}
