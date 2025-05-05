using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Cinemachine;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class CameraController : NetworkBehaviour
{
  
    
    public CinemachineCamera cinemachine;
    //public override void OnStartAuthority() {
    //    Camera.SetActive(true);
    //}

    public void Update()
    {
        if (isLocalPlayer)
        {
            if (!cinemachine) { cinemachine = Object.FindAnyObjectByType<CinemachineCamera>(); }

            cinemachine.Follow = transform;
            

        }
        else
        {
            if (cinemachine) { cinemachine.gameObject.SetActive(false); }
        }



        //if (isLocalPlayer)
        //{
        //    if (!myCam){myCam = Camera.main;}

        //    //myCam.transform.SetParent(transform);

        //}
        //else 
        //{
        //    if(myCam) {myCam.gameObject.SetActive(false); }
        //}

    }

}
