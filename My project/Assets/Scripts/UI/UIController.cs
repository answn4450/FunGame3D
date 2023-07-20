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
    public Text playerStructPos;
    public Text playerStructType;
    public Text playerAttackType;
    public Text structureUseStat;

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

        playerStructPos.text = player.structPositionName.ToString();
        playerStructType.text = player.structTypeName.ToString();
        playerAttackType.text = player.attackTypeName.ToString();
        
        playerStructPos.GetComponent<Text>().color = player.useType == 0 ? Color.black : Color.gray;
        playerStructType.GetComponent<Text>().color = player.useType == 1 ? Color.black : Color.gray;
        playerAttackType.GetComponent<Text>().color = player.useType == 2 ? Color.black : Color.gray;

        structureUseStat.text = Status.GetInstance().structureUse.ToString() + " / " + Status.GetInstance().structureMaxUse.ToString();
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
