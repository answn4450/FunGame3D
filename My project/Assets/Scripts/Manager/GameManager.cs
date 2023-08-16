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
        groundManager.ResetGroundsStat();
    }

    void Update()
    {
        TestUpdate();
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stopGame = !stopGame;
            Time.timeScale = stopGame ? 0.0f : 1.0f;
            uiController.GamePause(stopGame);
        }

        if (prevElevator != null && prevElevator.IsWithPlayer())
            prevElevator.MovePlayer();

        if (nextElevator != null && nextElevator.IsWithPlayer())
            nextElevator.MovePlayer();

        if (enemyManager)
        {
            enemyManager.LifeCycle();
            if (player)
                enemyManager.FollowPlayer(player);
        }

        if (player)
        {
            player.LifeCycle();
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
                structureManager.LoopStructuresInFolder();

                player.ChangeSelectedStructureIndex();
                player.Command();
                player.CommandTurnEye();
                player.OnGround();
                if (player.rideBullet)
                    player.RideBullet();
                else
                {
                    player.OnGround();
                    player.CommandMoveBody();
                    player.SphereBySize();
                    player.WithAffectPower();
                }
                
                groundManager.ReactGrounds(player);
                gameCamera.BehindPlayer(10.0f);
                
            }

            UIControll();
        }
        */
        
        //Tools.GetInstance().TraceGroundBug("Player");
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

    public void TestUpdate()
    {
        Time.timeScale = 13.0f;
        List<string> testers = new List<string> {
            "dummy", 
            "dummy2", 
            "Player",
            "Player1",
            "Player2",
            "Enemy" 
        };

        Tools.GetInstance().AddGroundUpper(GameObject.Find("upper").transform);
        
        if (!Input.GetKey(KeyCode.Space))
            groundManager.ReactGrounds();

        foreach (string name in testers)
        {
            Tools.GetInstance().ImplementBug(name);
            Tools.GetInstance().TraceGroundBug(name);
        }
    }  
    
}
