using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void ButtonStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void ButtonTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}

    public void ButtonPickBall()
    {
        SceneManager.LoadScene("PickBall");
    }

    public void ButtonQuitGameProgram()
    {
        Application.Quit();
    }
}
