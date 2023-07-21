using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private CameraController gameCamera;
    private PlayerController player;
    private UIController uiController;

    public ElevatorController prevElevator;
    public ElevatorController nextElevator;
    
    private GroundController ground;

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

        GameObject playSet = GameObject.Find("PlaySet");
        player = playSet.transform.GetChild(1).gameObject.GetComponent<PlayerController>();
        gameCamera = player.transform.GetChild(0).gameObject.GetComponent<CameraController>();
        uiController = playSet.transform.GetChild(2).gameObject.GetComponent<UIController>();

        GameObject ground = GameObject.Find("Ground");
        Status.GetInstance().groundWidth = ground.transform.localScale.x;
        Status.GetInstance().groundHeight = ground.transform.localScale.z;

    }

    void Update()
    {
        if (prevElevator.WithPlayer() && prevElevator.arrive)
            SwitchScene(-1);

        if (prevElevator.WithPlayer() && nextElevator.arrive)
            SwitchScene(1);

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
            player.Move();
            player.Command();
            player.WithAffectPower();
            uiController.UIPlay(player);
            gameCamera.BehindPlayer(10.0f);
        }
    }

    public void SwapScene()
    {

    }

    private void SwitchScene(int step)
	{
        if (Status.GetInstance().currentStage + step > Status.GetInstance().maxStage)
            Status.GetInstance().endGame = true;
        else
        {
            Status.GetInstance().currentStage += step;
            SceneManager.LoadScene("Stage" + Status.GetInstance().currentStage.ToString());
        }
	}
}
