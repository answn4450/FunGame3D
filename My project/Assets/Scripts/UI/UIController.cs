using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Text TextPlayerSize;
    public Text TextCountdown;
    public Text playerPosition;
    public Text playerPointPosition;

    public GameObject DeadUI;

    private void Awake()
    {
        DeadUI.SetActive(false);
    }

    public void UIPlay(PlayerController player)
    {
        TextPlayerSize.text = player.size.ToString();
        playerPosition.text = ((int)player.transform.position.x).ToString().ToString()
            + "," + ((int)player.transform.position.z).ToString().ToString();
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
