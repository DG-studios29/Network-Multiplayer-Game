using UnityEngine;

public class CannonFire : MonoBehaviour
{

    [SerializeField] private int index;


    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIndex(int idx)
    {
        index = idx;
    }


    public int GetIndex()
    {
        return index;
    }

    public void FireProjectile()
    {
        Debug.Log("Look who finally made some progress " + this.name);
    }

}
