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
    public SkyPannelController skyPannel;

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
        
        groundManager.CreateGrounds();
        skyPannel.SetSkyPannelByGroundPannel(groundManager);
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


        uiController.SetUI(player);
    }

    void Update()
    {
        groundManager.BeforeCycle();

        if (Input.GetKeyDown(KeyCode.Escape) && !player.dead)
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
            skyPannel.TransparentByPlayerY(player);
            structureManager.LoopStructuresInFolder();

            player.SetInput();
            player.ChangeSelectedStructureIndex();
            player.CommandTurnEye();
            player.Command();

            if (player.rideBullet)
                player.RideBullet();
            else
            {
                player.OnGround();
                player.CommandMoveBody();
                player.WithAffectPower();
                player.RollPaint();
                player.HealByPlayerGround();

                GroundController playerUnderGround = Tools.GetInstance().GetUnderGround(player.transform);
                if (!(playerUnderGround.NeedSqueeze() && player.IsSizeBigger()))
                    player.BackToSize();
            }

            groundManager.ReactGrounds();
            if (!player.rideBullet)
                groundManager.SqueezePlayer(player);
            
            gameCamera.BehindPlayer(10.0f);
            gameCamera.CleanBlockView();
        }

        UIControll();

        //Tools.GetInstance().Test();
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
