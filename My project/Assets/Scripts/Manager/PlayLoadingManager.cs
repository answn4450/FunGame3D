using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayLoadingManager : MonoBehaviour
{
    private AsyncOperation asyncOperation;
    public Text loadingPercent;
    public Text nextStage;
    //public Image progressImage;

    IEnumerator Start()
    {
        if (Status.GetInstance().gotPlaySet == false)
        {
            Status.GetInstance().gotPlaySet = true;

            asyncOperation = SceneManager.LoadSceneAsync("Stage" + Status.GetInstance().currentStage.ToString());
            nextStage.text = Status.GetInstance().currentStage.ToString() + "Ãþ";
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                //print(asyncOperation.progress);
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                loadingPercent.text = (progress * 100f).ToString() + "%";
                //progressImage.GetComponent<Image>().fillAmount = progress;
                yield return null;

                if (progress > 0.7f)
                {
                    if (Input.GetKey(KeyCode.Space))
                        asyncOperation.allowSceneActivation = true;
                }
            }
        }
    }
}