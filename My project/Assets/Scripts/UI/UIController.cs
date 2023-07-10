using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Text TextPlayerSize;
    public Text TextCountdown;

    public GameObject DeadUI;

    public void UIPlay(PlayerController player)
    {
        TextPlayerSize.text = player.size.ToString();
        DeadUI.SetActive(false);
    }

    public void DeadCountdown(float countdown)
    {
        if (countdown > 0)
        {
            TextCountdown.text = ((int)(countdown + 0.9f)).ToString();
        }

        DeadUI.SetActive(true);
    }
}
