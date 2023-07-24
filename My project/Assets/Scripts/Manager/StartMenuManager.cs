using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public PlayerController player;
    //public CameraController mainCamera;

    void Update()
    {
        player.Move();
    }

    public void ButtonPlay()
    {
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }

    private void SetUpPlay()
    {

    }
}
