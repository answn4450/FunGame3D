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
    public EnemyManager enemyManager;

    private CameraController gameCamera;
    private PlayerController player;
    private PlayerEyeController playerEye;
    private UIController uiController;
    private StructureManager structureManager;
    
    private Transform builtStructureFolder;
    
    private const float maxCountdown = 10.0f;
    private float leftCountdown;

    private bool stopGame;

    void Awake()
    {
        leftCountdown = maxCountdown;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stopGame = !stopGame;
            Time.timeScale = stopGame ? 0.0f : 1.0f;
            uiController.GamePause(stopGame);
        }

        groundManager.ResetGroundsStat();

        if (prevElevator != null && prevElevator.IsWithPlayer())
            prevElevator.MovePlayer();

        if (nextElevator != null && nextElevator.IsWithPlayer())
            nextElevator.MovePlayer();

        if (enemyManager)
        {
            if (player)
                enemyManager.FollowPlayer(player);
            enemyManager.LifeCycle();
        }

        if (player)
        {
            if (player.dead)
            {
                leftCountdown -= Time.deltaTime;
                if (leftCountdown > 0)
                    uiController.DeadCountdown(leftCountdown);
                else
                    SceneManager.LoadScene("StartMenu");

                gameCamera.AroundPoint(leftCountdown / maxCountdown * 360.0f);
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
        uiController.PlayerRebirth();
        if (player)
            player.Rebirth();
        leftCountdown = maxCountdown;
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
