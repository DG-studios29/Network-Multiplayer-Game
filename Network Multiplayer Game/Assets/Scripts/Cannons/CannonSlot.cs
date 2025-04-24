using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CannonSlot : MonoBehaviour
{

    [SerializeField] private int index; //identify our cannon
    [SerializeField] private int loadNo; //number it will be fired out in our sequence

    [SerializeField]private TMP_Text loadText;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IndexSet(int idx)
    {
        index = idx;
    
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



}
