using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public PlayerController player;
    public CameraController playCam;
    public GroundManager ground;
    public Text finish;

	private void Start()
	{
        Ground.GetInstance().SetGround(ground);
	}

	void Update()
    {
        playCam.AroundPoint(10.0f);
        finish.text = Status.GetInstance().endGame ? "완료 했음" : "완료 필요";
        player.CommandMoveBody();
        player.CommandTurnEye();
    }

    public void ButtonPlay()
    {
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }
}
