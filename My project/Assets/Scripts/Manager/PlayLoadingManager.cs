using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayLoadingManager : MonoBehaviour
{
    public Text nextStage;
    public GameObject playSet;

    void Update()
    {
        nextStage.text = Status.GetInstance().currentStage.ToString() + "Ãþ";
        if (Input.GetKey(KeyCode.Space))
            Play();
    }

    private void Play()
    {
        playSet.SetActive(true);
        int loadStage = Status.GetInstance().currentStage;
        SceneManager.LoadScene("Stage" + loadStage.ToString());
    }
}
