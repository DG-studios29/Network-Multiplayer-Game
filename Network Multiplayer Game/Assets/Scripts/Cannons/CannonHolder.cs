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

    [SerializeField]private List<CannonSlot> loadedCannons; //will update itself according to our setup

    private CannonSlot ActiveSelectedBtn;

    [SerializeField] private CannonLinq cannonLinq;

    private int firstMatch = 0;
    int firstAppear,lastAppear;

    //will need to know selected
    [SerializeField] private PresetsHolder presetsHolder;
    private PresetsBtn selectedPresetBtn;
    private LoadPresetData presetData;

    [SerializeField] private NetTempFinder netTempFinder;


    private void Awake()
    {
        cSlots = new CannonSlot[6]; //create cannon slot array of six 

        loadedCannons = new List<CannonSlot>();

        if(netTempFinder == null)
        {

        }
        else
        {
            cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
            presetsHolder = GameObject.FindAnyObjectByType<PresetsHolder>();
        }
    

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
            if (cannonLinq.CanFireCannon())
             {

                Debug.Log("Called From Cannon Holder");

                //call Cannon Linq
                cannonLinq.FireCannonCalled(loadedCannons[0].GetSlotIndex());


                //Case of multiples
                if(CountMultiples(loadedCannons[0]) > 1)
                {
                    loadedCannons[0].TakeMulti();
                }
                else
                {
                    loadedCannons[0].ResetCannon();
                }
                

                loadedCannons.Remove(loadedCannons[0]);  //remove the first in
                                                         //update the counts on the remaining cannons
                UpdateLoadCount();
            }
            else
            {
                Debug.Log("Wait on Load");
            }

        }
        
    }

    public void AddToLoadedList()
    {
        ActiveSelectedBtn = UseSelectedCannonBtn();

        if (loadedCannons.Count < 6)
        {

            if (ActiveSelectedBtn != null)
            {
                //needs to check if its already in the list
                if (!CheckAlreadyInList(ActiveSelectedBtn))
                {
                    loadedCannons.Add(ActiveSelectedBtn);
                    ActiveSelectedBtn.SetCannon(loadedCannons.Count); //number

                    ActiveSelectedBtn = null;

                    cannonLinq.LinkHUD();
                }
                else
                {
                    Debug.Log("Adding on top");
                    loadedCannons.Add(ActiveSelectedBtn);

                    //Get number of repetitions and set our multi text
                    ActiveSelectedBtn.SetMultiCount(CountMultiples(ActiveSelectedBtn));

                    ActiveSelectedBtn = null;

                    //not sure if we'll need to sync HUD
                }



            }
            else
            {
                Debug.Log("Nothing has been selected");

            }
        }
        else
        {
            Debug.Log("Our List is full and we can no longer add anything");
        }
    }

    public void RemoveFromLoadedList()
    {
        ActiveSelectedBtn = UseSelectedCannonBtn();

        if(ActiveSelectedBtn != null)
        {
            if (CheckAlreadyInList(ActiveSelectedBtn))
            {
                //needs to know if we'll clear multiples?
                if (CountMultiples(ActiveSelectedBtn) > 1)
                {
                    //remove an occurance. first occurance
                    //Remove first


                    loadedCannons.Remove(loadedCannons[firstMatch]); //remove the first match in list

                    ActiveSelectedBtn.SetMultiCount(CountMultiples(ActiveSelectedBtn));

                    UpdateLoadCount();

                    ActiveSelectedBtn = null;

                }
                else
                {

                    loadedCannons.Remove(ActiveSelectedBtn);
                    ActiveSelectedBtn.ResetCannon();

                    UpdateLoadCount();

                    ActiveSelectedBtn = null;
                }
            }
            else
            {
                Debug.Log("This does not exist in our list. Cant be cleared");
            }
           
        }
    }


    public void UsePresetToLoad()
    {
        //start fresh
        ClearLoadedList();
        CannonSlot loadingSlot = null;

        if (loadedCannons.Count < 6)
        {


            //build the sequence
            selectedPresetBtn = presetsHolder.FindActivePreset();

            if (selectedPresetBtn != null)
            {
                presetData = selectedPresetBtn.FindPresetData();
                

                for (int i = 0; i < cSlots.Length; i++)
                {
                    int seqValue = presetData.GetBuildSequence(i);
                    //Debug.Log(" Sequence " + seqValue.ToString());

                    foreach(CannonSlot slot in cSlots)
                    {
                        if (slot.GetSlotIndex() == seqValue)
                        {
                            loadingSlot = slot;
                            break;
                        }
                    }

                    if (seqValue > 0 && loadingSlot != null)
                    {
                        if (!CheckAlreadyInList(loadingSlot))
                        {
                            loadedCannons.Add(loadingSlot);
                            loadingSlot.SetCannon(loadedCannons.Count); //number

                            loadingSlot = null;

                            cannonLinq.LinkHUD();
                        }
                        else
                        {
                            Debug.Log("Adding on top");
                            loadedCannons.Add(loadingSlot);

                            //Get number of repetitions and set our multi text
                            loadingSlot.SetMultiCount(CountMultiples(loadingSlot));

                            loadingSlot = null;

                            //not sure if we'll need to sync HUD
                        }
                    }
                }

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
                //int appears = CountMultiples(loadedCannons[i]);

                //check if our cannon reappears
                if(CountMultiples(loadedCannons[i]) > 1)
                {
                    if (CheckAlreadyInList(loadedCannons[i]))
                    {
                        loadedCannons[i].FirstMatchMarked(firstMatch + 1);
                        loadedCannons[i].SetCannon(firstMatch + 1);

                    }
                }
                
                else
                {
                    loadedCannons[i].SetCannon(i + 1);
                }

            }
        }

        cannonLinq.LinkHUD();


    }

    private bool CheckAlreadyInList(CannonSlot checkingSlot)
    {
        if (loadedCannons.Count > 0)
        {
            for (int i = 0; i < loadedCannons.Count; i++)
            {
                if (checkingSlot.GetSlotIndex() == loadedCannons[i].GetSlotIndex())
                {
                    firstMatch = i; //first match is first index of this
                    return true;
                }
            }
        }
        return false;
    }


    private int CountMultiples(CannonSlot checkingSlot)
    {
        int occurs = 0;
        for(int i = 0; i < loadedCannons.Count; i++)
        {
            if(checkingSlot.GetSlotIndex() == loadedCannons[i].GetSlotIndex())
            {
                occurs++;
            }
        }

        return occurs;
    }
    


    public void HandleMulti()
    {

    }

    public void ClearLoadedList()
    {
        loadedCannons.Clear();


        foreach(CannonSlot slot in cSlots)
        {
            slot.ResetCannon();
        }
        cannonLinq.LinkHUD();
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

    public CannonSlot LinkSlot(int i)
    {
        return cSlots[i];
    }

    public int CountLoadedCannons()
    {
        return loadedCannons.Count;
    }


}
