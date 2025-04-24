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

    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      MakeCannonArray();
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void FireLoadedCannon()
    {
        if(loadedCannons.Count <= 0)
        {
            Debug.Log("Cannot fire anything, need to reload");
            return;
        }
        else
        {
            loadedCannons.Remove(loadedCannons[0]);  //remove the first in
            //update the counts on the remaining cannons
        }
    }

    void AddToLoadedList()
    {

    }

    void ClearLoadedList()
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






}
