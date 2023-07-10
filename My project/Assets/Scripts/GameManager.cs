using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    public CameraController camera;
    public PlayerController player;
    public UIController uiController;

    private float NewCountdown;
    private float LeftCountdown;


    void Awake()
    {
        NewCountdown = 10.0f;
        LeftCountdown = NewCountdown;
    }

    void Update()
    {
        uiController.UIPlay(player);

        if (player.dead)
        {
            LeftCountdown -= Time.deltaTime;
            if (LeftCountdown > 0)
                uiController.DeadCountdown(LeftCountdown);
            else
                SceneManager.LoadScene("StartMenu");
            
            camera.Revolution(LeftCountdown/NewCountdown * 360.0f);
        }
        else
        {
            camera.BehindPlayer(10.0f);
        }
    }
}
