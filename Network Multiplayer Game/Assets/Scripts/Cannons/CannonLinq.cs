using UnityEngine;
using System.Collections.Generic;
using System;

public class CannonLinq : MonoBehaviour
{

    private CannonFire[] realCannons;

    [SerializeField] private CannonFire cannon1;
    [SerializeField] private CannonFire cannon2;
    [SerializeField] private CannonFire cannon3;
    [SerializeField] private CannonFire cannon4;
    [SerializeField] private CannonFire cannon5;
    [SerializeField] private CannonFire cannon6;


    //public List<CannonSlot> cSlots;
    //public List<CannonFire> cFires;

    private CannonHolder cannonHolder;

    private void Awake()
    {
        realCannons = new CannonFire[6];

        cannonHolder = GameObject.FindAnyObjectByType<CannonHolder>();//Cannon GUI needs to be active at this point
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //realCannons = GetComponentsInChildren<CannonFire>(); 

        MakeRealCannonArray();

        if(cannonHolder == null)
        {
            Debug.Log("Couldnt find");
        }
        else
        {
            Debug.Log("Found it");
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void MakeRealCannonArray()
    {
        realCannons[0] = cannon1;
        realCannons[1] = cannon2;
        realCannons[2] = cannon3;
        realCannons[3] = cannon4;
        realCannons[4] = cannon5;
        realCannons[5] = cannon6;

        for(int i = 0; i < 6; i++)
        {
            realCannons[i].SetIndex(i + 1);
        }


    }


    public void FireCannonChain()
    {
        Debug.Log("Called From Cannon Linq");
        cannonHolder.FireLoadedCannon();
    }

    public void FireCannonCalled(int callIndex)
    {
        for(int i = 0; i < realCannons.Length; i++)
        {
            if(realCannons[i].GetIndex() == callIndex)
            {
                realCannons[i].FireProjectile();
            }
        }
    }



}
