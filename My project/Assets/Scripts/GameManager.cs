using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    public CameraController Camera;
    private PlayerController Player;
    
    private float NewCountdown;
    private float LeftCountdown;

    public GameObject DeadUI;
    public Text TextCountdown;

    void Awake()
    {
        NewCountdown = 10.0f;
        LeftCountdown = NewCountdown;
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Player.dead)
        {
            DeadUI.SetActive(true);
            TextCountdown.text = ((int)(LeftCountdown + 0.9f)).ToString();
            LeftCountdown -= Time.deltaTime;

            Camera.SetTransformByY(LeftCountdown/NewCountdown*360.0f);
		}
        DeadUI.SetActive(false);
    }
}
