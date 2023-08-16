using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
	public void ButtonTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}

    public void ButtonQuitGameProgram()
    {
        Application.Quit();
    }
}
