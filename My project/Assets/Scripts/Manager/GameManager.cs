using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ElevatorController prevElevator;
    public ElevatorController nextElevator;
    public GroundManager groundManager;

    private CameraController gameCamera;
    private PlayerController player;
    private PlayerEyeController playerEye;
    private UIController uiController;
    private StructureManager structureManager;
    
    private Transform builtStructureFolder;
    
    private float newCountdown;
    private float leftCountdown;

    void Awake()
    {
        newCountdown = 10.0f;
        leftCountdown = newCountdown;

        Ground.GetInstance().SetGround(groundManager);

        builtStructureFolder = new GameObject("Built Structure Folder").transform;
        structureManager = builtStructureFolder.gameObject.AddComponent<StructureManager>();
        
    }
    
    private void Start()
    {
        transform.position = Vector3.zero;
        
        GameObject playSet = GameObject.Find("PlaySet");
        GameObject playerSet = GameObject.Find("PlayerSet");
        
        uiController = playSet.transform.GetChild(2).gameObject.GetComponent<UIController>();
        
        player = playerSet.transform.GetChild(0).gameObject.GetComponent<PlayerController>();
        player.structureFolder = builtStructureFolder;
        playerEye = playerSet.transform.GetChild(1).gameObject.GetComponent<PlayerEyeController>();
        gameCamera = playerEye.transform.GetChild(0).gameObject.GetComponent<CameraController>();

        groundManager.CreateGrounds();

        uiController.SetUI(player);
    }

    private void FixedUpdate()
    {
        groundManager.BeforeTemporailyAffect();
    }

    void Update()
    {
        groundManager.ResetGroundsStat();
        if (prevElevator != null && prevElevator.IsWithPlayer())
            prevElevator.MovePlayer();

        if (nextElevator != null && nextElevator.IsWithPlayer())
            nextElevator.MovePlayer();

        //structureManager.Manage();
        if (player)
        {
            if (player.dead)
            {
                leftCountdown -= Time.deltaTime;
                if (leftCountdown > 0)
                    uiController.DeadCountdown(leftCountdown);
                else
                    SceneManager.LoadScene("StartMenu");

                gameCamera.AroundPoint(leftCountdown / newCountdown * 360.0f);
            }
            else
            {
                player.OnGround();
                structureManager.LoopStructuresInFolder();

                groundManager.ReactGrounds(player);

                player.SphereBySize();
                player.ChangeSelectedStructureIndex();
                player.Command();
                player.CommandTurnEye();

                if (player.rideBullet)
                    player.RideBullet();
                else
                {
                    player.CommandMoveBody();
                    player.WithAffectPower();
                }
                
                gameCamera.BehindPlayer(10.0f);
                
            }

            UIControll();
        }
    }

    public void Rebirth()
	{
        if (player)
            player.Rebirth();
	}

    private void UIControll()
    {

        uiController.UIPlay(player);
        if (nextElevator && player)
            uiController.NextElevatorDistanceInfo(
                nextElevator.transform.position - player.transform.position
                );
    }
}
