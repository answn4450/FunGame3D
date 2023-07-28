using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ManualKey;

public class GameManager : MonoBehaviour
{
    public ElevatorController prevElevator;
    public ElevatorController nextElevator;
    public GroundManager ground;
    private CameraController gameCamera;
    private PlayerController player;
    private PlayerEyeController playerEye;
    private UIController uiController;

    private float newCountdown;
    private float leftCountdown;

    void Awake()
    {
        newCountdown = 10.0f;
        leftCountdown = newCountdown;
        Status.GetInstance().structureUse = 0;
        Status.GetInstance().structureMaxUse = 5;
        Status.GetInstance().groundX0 = ground.transform.position.x - ground.transform.localScale.x * 0.5f;
        Status.GetInstance().groundX1 = ground.transform.position.x + ground.transform.localScale.x * 0.5f;
        Status.GetInstance().groundZ0 = ground.transform.position.z - ground.transform.localScale.z * 0.5f;
        Status.GetInstance().groundZ1 = ground.transform.position.z + ground.transform.localScale.z * 0.5f;
        Status.GetInstance().groundY = ground.transform.position.y + 0.5f;
    }
    
    private void Start()
    {
        transform.position = Vector3.zero;
        
        GameObject playSet = GameObject.Find("PlaySet");
        GameObject playerSet = GameObject.Find("PlayerSet");
        
        uiController = playSet.transform.GetChild(2).gameObject.GetComponent<UIController>();
        
        player = playerSet.transform.GetChild(0).gameObject.GetComponent<PlayerController>();
        playerEye = playerSet.transform.GetChild(1).gameObject.GetComponent<PlayerEyeController>();
        gameCamera = playerEye.transform.GetChild(0).gameObject.GetComponent<CameraController>();
    }

    void Update()
    {
        if (prevElevator != null && prevElevator.IsWithPlayer())
            prevElevator.MovePlayer();

        if (nextElevator != null && nextElevator.IsWithPlayer())
            nextElevator.MovePlayer();

        if (player)
        {
            uiController.UIPlay(player);
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
                ground.UpDownOrSqueeze(player);
                
                player.Command();
                player.Move();

                if (player.rideBullet)
                    player.RideBullet();
                else
                    player.WithAffectPower();
                
                gameCamera.BehindPlayer(10.0f);
                player.BindPosition();
            }
        }
    }
}
