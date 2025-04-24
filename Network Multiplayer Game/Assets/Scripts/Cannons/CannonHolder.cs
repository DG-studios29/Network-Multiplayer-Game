using UnityEngine;
using System.Collections.Generic;
using System;

public class CannonHolder : MonoBehaviour
{

    [SerializeField]private CannonSlot cannon1;
    [SerializeField]private CannonSlot cannon2;
    [SerializeField] private CannonSlot cannon3;
    [SerializeField] private CannonSlot cannon4;
    [SerializeField] private CannonSlot cannon5;
    [SerializeField] private CannonSlot cannon6;

    private CannonSlot[] cSlots;

    private List<CannonSlot> loadedCannons; //will update itself according to our setup

    private CannonSlot ActiveSelectedBtn;

    private CannonLinq cannonLinq;


    private void Awake()
    {
        cSlots = new CannonSlot[6]; //create cannon slot array of six 

        loadedCannons = new List<CannonSlot>();

        cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeCannonArray();
        ActiveSelectedBtn = null;

      
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void FireLoadedCannon()
    {
        if(loadedCannons.Count <= 0)
        {
            Debug.Log("Cannot fire anything, need to reload");
            return;
        }
        else
        {
            Debug.Log("Called From Cannon Holder");

            //call Cannon Linq
            cannonLinq.FireCannonCalled(loadedCannons[0].GetSlotIndex());

            loadedCannons[0].ResetCannon();

            loadedCannons.Remove(loadedCannons[0]);  //remove the first in
            //update the counts on the remaining cannons
            UpdateLoadCount();

          

          
        }
    }

    public void AddToLoadedList()
    {
        ActiveSelectedBtn = UseSelectedCannonBtn();

        if(ActiveSelectedBtn != null)
        {
            //needs to check if its already in the list
            if (!CheckAlreadyInList(ActiveSelectedBtn))
            {
                loadedCannons.Add(ActiveSelectedBtn);
                ActiveSelectedBtn.SetCannon(loadedCannons.Count); //number

                ActiveSelectedBtn = null;
            }
            else
            {
                Debug.Log("Already in Sequence");
            }


           
        }
        else
        {
            Debug.Log("Nothing has been selected");
        }
    }

    public void RemoveFromLoadedList()
    {
        ActiveSelectedBtn = UseSelectedCannonBtn();

        if(ActiveSelectedBtn != null)
        {
            if (CheckAlreadyInList(ActiveSelectedBtn))
            {
                loadedCannons.Remove(ActiveSelectedBtn);
                ActiveSelectedBtn.ResetCannon();

                UpdateLoadCount();

                ActiveSelectedBtn = null;
            }
            else
            {
                Debug.Log("This does not exist in our list. Cant be cleared");
            }
           
        }
    }

    private void UpdateLoadCount()
    {
        if (loadedCannons.Count == 0)
        {
            ClearLoadedList();
        }
        else
        {

            for (int i = 0; i < loadedCannons.Count; i++)
            {
                loadedCannons[i].SetCannon(i + 1);
            }
        }
    }

    bool CheckAlreadyInList(CannonSlot checkingSlot)
    {
        if(loadedCannons.Count == 0)
        {
            return false;
        }

        for(int i = 0; i < loadedCannons.Count; i++)
        {
            if(checkingSlot == loadedCannons[i])
            {
                return true;
            }
        }

        return false;
    }


    public void ClearLoadedList()
    {
        loadedCannons.Clear();
    }



    void MakeCannonArray()
    {
        cSlots[0] = cannon1;
        cSlots[1] = cannon2;
        cSlots[2] = cannon3;
        cSlots[3] = cannon4;
        cSlots[4] = cannon5;
        cSlots[5] = cannon6;


        for(int i = 0; i < cSlots.Length; i++)
        {
            cSlots[i].IndexSet(i + 1);
        }

    }

    //Useful for navigating to different UIs
    private void CannonBtnsDisableAll()
    {
        for(int i = 0; i < cSlots.Length; i++)
        {
            cSlots[i].CannonDisable();
        }
    }

    private void CannonBtnsEnableAll()
    {
        for (int i = 0; i < cSlots.Length; i++)
        {
            cSlots[i].CannonEnable();
        }
    }

    private CannonSlot UseSelectedCannonBtn()
    {
        for(int i = 0; i < cSlots.Length; i++)
        {
            if (cSlots[i].CheckBtnSelection())
            {
                return cSlots[i];
            }
            
        }

        return null;
    }






}
