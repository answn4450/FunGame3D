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

        player.SetInput();
        player.Command();
        player.LifeCycle();
        
        player.CommandTurnEye();
        
        if (player.rideBullet)
            player.RideBullet();
        else
        {
            player.OnGround();
            player.CommandMoveBody();
            player.WithAffectPower();

            GroundController playerUnderGround = Tools.GetInstance().GetUnderGround(player.transform);
            if (true || !(playerUnderGround.NeedSqueeze() && player.IsSizeBigger()))
                player.BackToSize();
        }
        
        groundManager.ReactGrounds();
    }

    public void ButtonPlay()
    {
        SceneManager.LoadScene("PlayLoading");
    }
}
