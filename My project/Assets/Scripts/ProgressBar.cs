using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ProgressBar : MonoBehaviour
{
    private AsyncOperation asyncOperation;
    public Text text;
    public Text messagetext;
    public Image progressImage;

    IEnumerator Start()
    {
        asyncOperation = SceneManager.LoadSceneAsync("Stage" + Status.GetInstance().currentStage.ToString());
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //print(asyncOperation.progress);
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            text.text = (progress * 100f).ToString() + "%";
            progressImage.GetComponent<Image>().fillAmount = progress;
            yield return null;

            if (progress > 0.7f)
            {
                messagetext.gameObject.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                    asyncOperation.allowSceneActivation = true;
            }
        }
    }
}