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

        float groundWidth = 15.0f;
        float groundHeight = 15.0f;
        GameObject groundObj = Instantiate(PrefabManager.GetInstance().GetPrefabByName("Ground1x1"));
        groundObj.name = "Ground";
        ground = groundObj.GetComponent<GroundController>();
        ground.SetSize(groundWidth, groundHeight);
        Status.GetInstance().groundWidth = groundWidth;
        Status.GetInstance().groundHeight = groundHeight;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            SwitchScene();

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
        {
            Status.GetInstance().endGame = true;
            SceneManager.LoadScene("StartMenu");
        }
	}
}
