using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public PlayerController player;
    public GroundManager ground;
    public Text finish;

	private void Start()
	{
        Ground.GetInstance().SetGround(ground);
        finish.text = Status.GetInstance().endGame ? "완료 했음" : "완료 필요";
	}

	void Update()
    {
        player.LifeCycle();
        player.CommandMoveBody();
        player.CommandTurnEye();
        player.WithAffectPower();
    }

    public void ButtonPlay()
    {
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }
}
