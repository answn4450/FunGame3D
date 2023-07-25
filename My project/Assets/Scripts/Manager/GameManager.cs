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
    public GameObject ground;
    private CameraController gameCamera;
    private PlayerController player;
    private UIController uiController;

    private float newCountdown;
    private float leftCountdown;

    void Awake()
    {
        newCountdown = 10.0f;
        leftCountdown = newCountdown;
    }
    
    private void Start()
    {
        Status.GetInstance().structureUse = 0;
        Status.GetInstance().structureMaxUse = 5;
        Status.GetInstance().groundX0 = ground.transform.position.x - ground.transform.localScale.x * 0.5f;
        Status.GetInstance().groundX1 = ground.transform.position.x + ground.transform.localScale.x * 0.5f;
        Status.GetInstance().groundZ0 = ground.transform.position.z - ground.transform.localScale.z * 0.5f;
        Status.GetInstance().groundZ1 = ground.transform.position.z + ground.transform.localScale.z * 0.5f;
        Status.GetInstance().groundY = ground.transform.position.y + 0.5f;
        
        GameObject playSet = GameObject.Find("PlaySet");
        player = playSet.transform.GetChild(1).gameObject.GetComponent<PlayerController>();
        gameCamera = player.transform.GetChild(0).gameObject.GetComponent<CameraController>();
        uiController = playSet.transform.GetChild(2).gameObject.GetComponent<UIController>();
    }

    void Update()
    {
        if (prevElevator != null && prevElevator.IsWithPlayer())
            SwitchScene(-1);

        if (nextElevator != null && nextElevator.IsWithPlayer())
            SwitchScene(1);

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
                player.Command();
                player.Move();
                gameCamera.BehindPlayer(10.0f);
                if (player.rideBullet)
                    player.RideBullet();
                else
                    player.WithAffectPower();
            }
        }
    }


    private void SwitchScene(int step)
	{
        int nextStage = Status.GetInstance().currentStage + step;
        if (nextStage >= Status.GetInstance().maxStage)
        {
            Status.GetInstance().endGame = true;
            SceneManager.LoadScene("StartMenu");
            Destroy(gameObject);
        }
        else if (nextStage >= 0)
        {
            Status.GetInstance().currentStage = nextStage;
            SceneManager.LoadScene("Stage" + nextStage.ToString());
        }
	}
}
