using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    public CameraController playCam;
    public Text finish;

	void Update()
    {
        playCam.AroundPoint(10.0f);
        finish.text = Status.GetInstance().endGame ? "완료 했음" : "완료 필요";
    }
}
