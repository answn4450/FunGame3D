using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public CameraController playCam;

    void Update()
    {
        playCam.AroundPoint(10.0f);    
    }
}
