using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public CameraController gameCamera;
    public PlayerController player;
    public UIController uiController;
    public GameObject elevator;

    public int stage;

    private float newCountdown;
    private float leftCountdown;

    private float groundWidth;
    private float groundHeight;
    void Awake()
    {
        newCountdown = 10.0f;
        leftCountdown = newCountdown;
        groundWidth = 5.0f;
        groundHeight = 5.0f;
    }

    void Update()
    {
        if ((elevator.transform.position - player.transform.position).magnitude < 1.0f)
            SwitchScene();

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
            //player.BindPosition(groundWidth * 0.5f, groundHeight * 0.5f);
            uiController.UIPlay(player);
            gameCamera.BehindPlayer(10.0f);
        }
    }

    private void SwitchScene()
	{
        if (stage == 0)
            Status.GetInstance().endGame = true;
	}
}
