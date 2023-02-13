using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAssigner : MonoBehaviour
{
    private GameObject firstSceneCamera;
    
    private void Start()
    {
        firstSceneCamera = GameObject.Find("Main Camera Back");

    }

    private void Update () {
        if( firstSceneCamera != null){
            this.transform.position = firstSceneCamera.transform.position;
            this.transform.rotation = firstSceneCamera.transform.rotation;
        }
    }
}
