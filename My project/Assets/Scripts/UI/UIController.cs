using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ManualKey;

public class UIController : MonoBehaviour
{
    public Text TextPlayerSize;
    public Text TextCountdown;
    public Text spaceInfo;

    public GameObject DeadUI;

    private void Awake()
    {
        DeadUI.SetActive(false);
    }

    public void UIPlay(PlayerController player)
    {
        TextPlayerSize.text = player.size.ToString();

        DeadUI.SetActive(false);

        SpaceInfo();
    }

    public void DeadCountdown(float countdown)
    {
        if (countdown > 0)
        {
            TextCountdown.text = ((int)(countdown + 0.9f)).ToString();
        }

        DeadUI.SetActive(true);
    }

    private void SpaceInfo()
    {
        spaceInfo.text = Status.GetInstance().spaceKey.ToString();
    }
}
