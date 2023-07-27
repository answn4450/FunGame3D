using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{
    public bool goBase;
    public bool enableChange;
    public bool moveToUp;
    private bool withPlayer;

    private void Awake()
    {
        withPlayer = false;
        goBase = false;
        enableChange = false;
        moveToUp = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
            withPlayer = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            withPlayer = true;
        if (other.tag == "bullet" && enableChange)
            ChangeMode();
    }

    public bool IsWithPlayer()
    {
        return withPlayer;
    }

    private void ChangeMode()
	{
        goBase = !goBase;
    }

    public void MovePlayer()
	{
        if (goBase)
            MovePlayerToBase();
        else if (moveToUp)
            MovePlayerToUp();
        else if (!moveToUp)
            MovePlayerToDown();
    }

    private void MovePlayerToDown()
    {
        int nextStage = Status.GetInstance().currentStage - 1;
        if (nextStage <= 0)
            SceneManager.LoadScene("Base");
        else
		{
            Status.GetInstance().currentStage = nextStage;
            SceneManager.LoadScene("Stage" + nextStage.ToString());
		}
    }

    private void MovePlayerToUp()
    {
        int nextStage = Status.GetInstance().currentStage + 1;
        if (nextStage >= Status.GetInstance().maxStage)
        {
            Status.GetInstance().endGame = true;
            SceneManager.LoadScene("StartMenu");
        }
        else
		{
            Status.GetInstance().currentStage = nextStage;
            SceneManager.LoadScene("Stage" + nextStage.ToString());
		}
    }

    private void MovePlayerToBase()
    {
        SceneManager.LoadScene("Base");
    }
}
