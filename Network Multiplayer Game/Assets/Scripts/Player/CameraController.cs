using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class CameraController : NetworkBehaviour
{
    public Camera myCam;
    public Camera minimap;
    private Camera MainmapCam;
    //public override void OnStartAuthority()
    //{
    //    Camera.SetActive(true);
    //}

    private void Start()
    {
        GameObject Mainmap = GameObject.Find("MapCamera");
        MainmapCam = Mainmap.GetComponent<Camera>();
    }

    public void Update()
    {
        //if (isLocalPlayer)
        //{
        //    if (!cinemachine) { cinemachine = Object.FindAnyObjectByType<CinemachineCamera>(); }

        //    cinemachine.Follow = transform;


        //}
        //else
        //{
        //    if (cinemachine) { cinemachine.gameObject.SetActive(false); }
        //}



        if (isLocalPlayer)
        {
            if (!myCam) { myCam = Camera.main; }
            if (!minimap) { minimap = Camera.main; }
            if (!MainmapCam) { MainmapCam = Camera.main; }
            //myCam.transform.SetParent(transform);

        }
        else
        {
            if (myCam) { myCam.gameObject.SetActive(false); }
            if (minimap) { minimap.gameObject.SetActive(false); }
            if (MainmapCam) { MainmapCam.gameObject.SetActive(false); }
        }

        

    }

}
