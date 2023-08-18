using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    public PlayerController player;
    public GroundManager groundManager;
    public Text finish;

    private void Awake()
    {
        Ground.GetInstance().SetGround(groundManager);
    }
        
    private void Start()
	{
        groundManager.CreateGrounds();
        finish.text = Status.GetInstance().endGame ? "완료 했음" : "완료 필요";
	}

	void Update()
    {
        groundManager.BeforeCycle();

        player.LifeCycle();
        player.OnGround();
        player.CommandMoveBody();
        player.CommandTurnEye();
        player.WithAffectPower();

        groundManager.ReactGrounds();
    }

    public void ButtonPlay()
    {
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }
}
