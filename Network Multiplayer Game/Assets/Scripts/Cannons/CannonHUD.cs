using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CannonHUD : MonoBehaviour
{
    private CannonLinq cannonLinq;
    [SerializeField]private CannonData ballType;

    [SerializeField]private TMP_Text ballName;
    [SerializeField]private Image ballImage;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cannonLinq = GameObject.FindAnyObjectByType<CannonLinq>();
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

}
