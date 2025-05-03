using UnityEngine;
using UnityEngine.UI;

public class PresetInput : MonoBehaviour
{
    [SerializeField]private Image background;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoldActivated()
    {
        background.color = Color.green;
    }

    public void HoldDropped()
    {
        background.color=Color.clear;
    }


    public void LoadUpPreset()
    {

    }


}
