using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CannonHUD : MonoBehaviour
{
    private CannonLinq cannonLinq;
    private CannonHolder cannonHolder;
    [SerializeField]private CannonData ballType;

    [SerializeField]private TMP_Text ballName;
    [SerializeField]private Image ballImage;
    
   
    [SerializeField] private LoadText[] ballTexts;
    [SerializeField] private LoadText ballTxt1;
    [SerializeField] private LoadText ballTxt2;
    [SerializeField] private LoadText ballTxt3;
    [SerializeField] private LoadText ballTxt4;
    [SerializeField] private LoadText ballTxt5;
    [SerializeField] private LoadText ballTxt6;

    bool linksAssigned = false;



    private void Awake()
    {
        cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
        


        ballTexts = new LoadText[6];

        ballTexts[0] = ballTxt1;
        ballTexts[1] = ballTxt2;
        ballTexts[2] = ballTxt3;
        ballTexts[3] = ballTxt4;
        ballTexts[4] = ballTxt5;
        ballTexts[5] = ballTxt6;
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cannonHolder = cannonLinq.CannonHolder; //lol

        linksAssigned = false;
        AssignLinks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SyncAmmoChange(CannonData cnData)
    {
        ballType = cnData;

        ballName.text = ballType.name.ToString();

    }

    public void SyncLoadDisplay()
    {
        if (!linksAssigned)
        {
            AssignLinks();
        }

        for (int i = 0; i < ballTexts.Length; i++)
         {
             ballTexts[i].CheckLoad();
         }
        
    }

    public void AssignLinks()
    {
        if (cannonHolder != null)
        {
            for (int i = 0; i < ballTexts.Length; i++)
            {
                ballTexts[i].AssignIndex(i + 1);


                ballTexts[i].AssignSlot(cannonHolder.LinkSlot(i));

            }

            linksAssigned = true;
        }
      

    }  

  

}
