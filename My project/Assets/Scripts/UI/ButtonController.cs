using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void ButtonPlay()
	{
		SceneManager.LoadScene("Play");
	}

	public void ButtonMenu()
	{
		SceneManager.LoadScene("StartMenu");
	}

	public void ButtonTutorial()
	{
		SceneManager.LoadScene("Tutorial");
	}
}
