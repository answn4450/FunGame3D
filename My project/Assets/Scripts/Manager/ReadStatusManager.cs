using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadStatusManager : MonoBehaviour
{
    void Start()
    {
        Status.GetInstance().Read();
        SceneManager.LoadScene("StartMenu");
    }
}
