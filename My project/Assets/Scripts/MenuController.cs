using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void ButtonPlay()
	{
		SceneManager.LoadScene("Play");
	}

	public void ButtonMenu()
	{
		SceneManager.LoadScene("StartMenu");
	}
}
