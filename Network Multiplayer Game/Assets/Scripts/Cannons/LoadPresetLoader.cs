using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class LoadPresetLoader : MonoBehaviour
{
    //This is gonna be hardcoded highkey, can be rebuilt in future
    [SerializeField] private LoadPresetData presetData;


    private TMP_Text[] sList;
    private TMP_Text[] mList;

    [SerializeField] private TMP_Text slotTxt1;
    [SerializeField] private TMP_Text slotTxt2;
    [SerializeField] private TMP_Text slotTxt3;
    [SerializeField] private TMP_Text slotTxt4;
    [SerializeField] private TMP_Text slotTxt5;
    [SerializeField] private TMP_Text slotTxt6;

    [SerializeField] private TMP_Text mP_Text1;
    [SerializeField] private TMP_Text mP_Text2;
    [SerializeField] private TMP_Text mP_Text3;
    [SerializeField] private TMP_Text mP_Text4;
    [SerializeField] private TMP_Text mP_Text5;
    [SerializeField] private TMP_Text mP_Text6;

    //Rebuild
    private int[] seqList;
    private List<int> sequencer = new List<int>();


    Dictionary<int, int> slotCountPair = new Dictionary<int, int>(); //just for display

    int firstIndex;

    int s1Count;
    int s2Count;
    int s3Count;
    int s4Count;
    int s5Count;
    int s6Count;


    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sList = new TMP_Text[6];
        mList = new TMP_Text[6];
        seqList = new int[6];


        presetData.SequenceInitialize();

        ArrayInitialiser();

        RepresentSequence();

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   /* public int SlotsGetGot(int k)
    {
        return presetData.SlotsIndexGetter(k);
    }

    public int MultiGetGot(int k)
    {
        return presetData.MultiIndexGetter(k);
    }*/

    private void ArrayInitialiser()
    {
     
       

        sList[0] = slotTxt1;
        sList[1] = slotTxt2;
        sList[2] = slotTxt3;
        sList[3] = slotTxt4;
        sList[4] = slotTxt5;
        sList[5] = slotTxt6;

        mList[0] = mP_Text1;
        mList[1] = mP_Text2;
        mList[2] = mP_Text3;
        mList[3] = mP_Text4;
        mList[4] = mP_Text5;
        mList[5] = mP_Text6;


        for(int i = 0; i < seqList.Length; i++)
        {
            seqList[i] = presetData.GetBuildSequence(i);
        }

        slotCountPair.Add(1,0);
        slotCountPair.Add(2,0);
        slotCountPair.Add(3,0);
        slotCountPair.Add(4,0);
        slotCountPair.Add(5, 0);
        slotCountPair.Add(6, 0);


        for(int i = 0; i < sList.Length; i++)
        {
            sList[i].text = "-";
            mList[i].text = "";
        }


    }



    void RepresentSequence()
    {
        for(int i = 0; i < seqList.Length; i++)
        {
            if (slotCountPair.ContainsKey(seqList[i]))
            {
                slotCountPair[seqList[i]]++;  //increments the value of the correspoding key

            }
        }

        for (int i = 0; i < sList.Length; i++)
        {
            if (slotCountPair[i + 1] > 1)
            {
                //maybe will need a first match

                //sList[i].text = presetData.SlotsIndexGetter(i).ToString();

                if (CheckSequence(i + 1))
                {
                    sList[i].text = firstIndex.ToString();
                }

                
            }
            else if(slotCountPair[i+1] == 1)
            {
                sList[i].text = (i + 1).ToString();
            }
            else
            {
                sList[i].text = "-";
            }

            if (slotCountPair[i +1] > 1)
            {
                mList[i].text = "x" + slotCountPair[i+1].ToString();
            }
            else
            {
                mList[i].text = "";
            }


        }


    }

    bool CheckSequence(int check)
    {
        for(int i = 0; i < seqList.Length; i++)
        {
            if(check == seqList[i])
            {
                //Debug.Log("was it really worth it");
                firstIndex = i +1;
                return true;

            }
            else
            {
                //Debug.Log("fault");
                //Debug.Log(seqList[i].ToString( )+ " AND " + check.ToString());

                
            }
        }
      
        return false;
    }


   /* void IndexGetterMethod()
    {
        if (presetData != null)
        {
            for (int i = 0; i < sList.Length; i++)
            {
                if (presetData.SlotsIndexGetter(i) != 0)
                {
                    sList[i].text = presetData.SlotsIndexGetter(i).ToString();
                }
                else
                {
                    sList[i].text = "#";
                }

                if (presetData.MultiIndexGetter(i) > 1)
                {
                    mList[i].text = "x" + presetData.SlotsIndexGetter(i).ToString();
                }
                else
                {
                    mList[i].text = "";
                }


            }

        }
    }*/

}
