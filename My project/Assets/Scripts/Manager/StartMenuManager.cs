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
        Ground.GetInstance().SetGroundWithPannel(ground);
	}

	void Update()
    {
        playCam.AroundPoint(10.0f);
        finish.text = Status.GetInstance().endGame ? "�Ϸ� ����" : "�Ϸ� �ʿ�";
        player.CommandMove();
    }

    public void ButtonPlay()
    {
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }
}
