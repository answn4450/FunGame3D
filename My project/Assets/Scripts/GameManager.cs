using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    public CameraController gameCamera;
    public PlayerController player;
    public UIController uiController;

    private float NewCountdown;
    private float LeftCountdown;

    private float groundWidth;
    private float groundHeight;

    void Awake()
    {
        NewCountdown = 10.0f;
        LeftCountdown = NewCountdown;
        groundWidth = 5.0f;
        groundHeight = 5.0f;
    }

    void Update()
    {
        if (player.dead)
        {
            LeftCountdown -= Time.deltaTime;
            if (LeftCountdown > 0)
                uiController.DeadCountdown(LeftCountdown);
            else
                SceneManager.LoadScene("StartMenu");

            gameCamera.AroundPoint(LeftCountdown/NewCountdown * 360.0f);
        }
        else
        {
            //player.BindPosition(groundWidth * 0.5f, groundHeight * 0.5f);
            uiController.UIPlay(player);
            gameCamera.BehindPlayer(10.0f);
        }
    }
}
