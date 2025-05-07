using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class CannonHUD : NetworkBehaviour
{
    [SerializeField]private CannonLinq cannonLinq;
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

    [SerializeField] private Image loadTimeImage;
    [SerializeField] private TMP_Text reloadText;

    int loadTextMessage;
    bool cannonsToLoad;

    [SerializeField] private GameObject netMessagesPopUp;
    [SerializeField] private TMP_Text netMessages; 
    [SerializeField] private TMP_Text netName;
    private string nameStr;

    
    //[SerializeField] private ScoreboardManager scoreboardManager;

    [SerializeField] private NetTempFinder netTempFinder;


    private void Awake()
    {
        if(netTempFinder == null)
        {

        }
        else
        {
            cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
        }
        
       
        ballTexts = new LoadText[6];

        ballTexts[0] = ballTxt1;
        ballTexts[1] = ballTxt2;
        ballTexts[2] = ballTxt3;
        ballTexts[3] = ballTxt4;
        ballTexts[4] = ballTxt5;
        ballTexts[5] = ballTxt6;
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //[ClientCallback]
    void Start()
    {
        if (!isLocalPlayer) return;

        cannonHolder = cannonLinq.CannonHolder; //lol
        

        linksAssigned = false;
        AssignLinks();
    }

    // Update is called once per frame
    //[ClientCallback]
    void Update()
    {
      if (!isLocalPlayer) return;

        LoadFillAmountUpdate();
        CheckListLength();
        
    }




    public void UIManage(string message)
    {
        //if(!isLocalPlayer) return;
        Debug.Log("Call made");
        StartCoroutine(ShowMessage(message));
    }

    public void UIName(string name)
    {
      //  if(!isLocalPlayer) return ;
        netName.text = name;

        ResetLocalName();
    }


  


    IEnumerator ShowMessage(string msg)
    {
        netMessagesPopUp.SetActive(true);
        netMessages.text = msg + "  " + netName.text;

        yield return new WaitForSeconds(15f);

        netMessages.text = " " ;
        netMessagesPopUp.SetActive(false);
    }


    public void ResetLocalName()
    {
        if (!isLocalPlayer) return;

        ScoreboardManager scoreboardManager = GetComponentInParent<ScoreboardManager>();
        if (scoreboardManager != null)
        {
            scoreboardManager.CmdSetPlayerName(GetCustomName());
        }
    }


    public string GetCustomName()
    {
        return netName.text;
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

    private void LoadFillAmountUpdate()
    {

        if(cannonLinq.LoadTimer >= cannonLinq.LoadTimeMax)
        {
            loadTimeImage.fillAmount = 1;
            if (cannonsToLoad)
            {
                reloadText.text = "Cannon Loaded";
                reloadText.color = Color.green;
            }
        }
        else
        {
            loadTimeImage.fillAmount = cannonLinq.LoadTimer / cannonLinq.LoadTimeMax;
            if (cannonsToLoad)
            {
                reloadText.text = "Loading Cannon";
                reloadText.color = Color.red;
            }

        }
   
    }


    public void CheckListLength()
    {
        if(cannonHolder.CountLoadedCannons() > 0)
        {
            cannonsToLoad = true;
            
        }
        else
        {
            cannonsToLoad = false;
            reloadText.text = "No Cannons Loading";
            reloadText.color = Color.yellow;
        }
    }


   

}
