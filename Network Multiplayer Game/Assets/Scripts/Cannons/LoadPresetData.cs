using UnityEngine;


[CreateAssetMenu(fileName = "LoadPresetData", menuName = "Scriptable Objects/LoadPresetData")]
public class LoadPresetData : ScriptableObject
{
    //poor approach
  /*  private int[] slotsSet; 
    private int[] multiSet;

    [SerializeField] int slots1;
    [SerializeField] int slots2;
    [SerializeField] int slots3;
    [SerializeField] int slots4;
    [SerializeField] int slots5;
    [SerializeField] int slots6;

    [SerializeField] int multi1;
    [SerializeField] int multi2;
    [SerializeField] int multi3;
    [SerializeField] int multi4;
    [SerializeField] int multi5;
    [SerializeField] int multi6;
*/
    //approach two
    private int[] sequence;

    [SerializeField] int seq1;
    [SerializeField] int seq2;
    [SerializeField] int seq3;
    [SerializeField] int seq4;
    [SerializeField] int seq5;
    [SerializeField] int seq6;

    public void SequenceInitialize()
    {
       /* slotsSet = new int[6];
        multiSet = new int[6];

        slotsSet[0] = slots1;
        slotsSet[1] = slots2;
        slotsSet[2] = slots3;
        slotsSet[3] = slots4;
        slotsSet[4] = slots5;
        slotsSet[5] = slots6;

        multiSet[0] = multi1;
        multiSet[1] = multi2;
        multiSet[2] = multi3;
        multiSet[3] = multi4;
        multiSet[4] = multi5;
        multiSet[5] = multi6;*/

        sequence = new int[6];

        sequence[0] = seq1;
        sequence[1] = seq2;
        sequence[2] = seq3;
        sequence[3] = seq4;
        sequence[4] = seq5;
        sequence[5] = seq6;


    }

   /* public int[] GetSlots()
    {
        return slotsSet;
    }

    public int[] GetMulti()
    {
        return multiSet;
    }*/

  /*  public int SlotsIndexGetter(int j)
    {
        return  slotsSet[j];
    }

    public int MultiIndexGetter(int j)
    {
        return multiSet[j];
    }*/

    public int GetBuildSequence(int i)
    {
        return sequence[i];
    }

}


