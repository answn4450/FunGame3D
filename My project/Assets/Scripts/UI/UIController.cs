using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Text textPlayerInSize;
    public Text textPlayerOutSize;
    public Text textCountdown;
    public Text underGroundEmptyHeight;
    public Text groundMinimumHeight;

    public GameObject deadUI;
    public GameObject pauseUI;
    public GameObject availableStructure;
    public GameObject builtStructure;
    public List<GameObject> availableStructureChild;
    public List<GameObject> builtStructureChild;

    private void Awake()
    {
        deadUI.SetActive(false);
        availableStructureChild = new List<GameObject>();
        builtStructureChild = new List<GameObject>();
    }

    public void SetUI(PlayerController player)
    {
        const float cellXY = 100;

        // builtStructure
        builtStructure.GetComponent<RectTransform>().sizeDelta = new Vector2(
            builtStructure.GetComponent<RectTransform>().sizeDelta.x,
            player.GetMaxStructure() * cellXY + (player.GetMaxStructure() - 1) * 10.0f
            );

        builtStructureChild = new List<GameObject>();
        GameObject builtStructureFirstChild = builtStructure.transform.GetChild(0).gameObject;
        
        
        for (int i = 0; i < player.GetMaxStructure(); ++i)
        {
            GameObject built;
            if (i == 0)
                built = builtStructureFirstChild;
            else
                built = Instantiate(
                builtStructureFirstChild,
                builtStructureFirstChild.transform.parent
                );

            built.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(
                builtStructure.GetComponent<RectTransform>().sizeDelta.x,
                cellXY     
            );

            builtStructureChild.Add(built);
            Destroy(built.GetComponent<VerticalLayoutGroup>());
        }

        // availableStructre
        List<string> availablePrefabNames = player.GetAvailableStructures();
        float size = availablePrefabNames.Count;
        availableStructure.GetComponent<RectTransform>().sizeDelta = new Vector2(
            cellXY * size,
            cellXY
            );

        for (int i= 0;i < size; ++i)
        {
            GameObject structureImage = Instantiate(availableStructure, availableStructure.transform);
            availableStructureChild.Add(structureImage);
            Destroy(structureImage.GetComponent<HorizontalLayoutGroup>());
            structureImage.GetComponent<Image>().sprite = Resources.Load("Images\\" +
                availablePrefabNames[i].ToString() + "Img"
                , typeof(Sprite)) as Sprite;
            structureImage.GetComponent<RectTransform>().sizeDelta = new Vector2(
                cellXY,
                cellXY
            );
        }
    }

    public void UIPlay(PlayerController player)
    {
        textPlayerInSize.text = EasyFloat(player.size);
        textPlayerOutSize.text = EasyFloat(player.transform.localScale.x);
        AvailableStructuresInfo(player.GetSelectedStructureIndex());
        BuiltStructuresInfo(player.GetBuiltStructures());
        MinimumGroundHeightInfo();
        UnderGroundEmptyHeightInfo(player.transform);
    }

    public void DeadCountdown(float countdown)
    {
        deadUI.SetActive(true);

        if (countdown > 0)
        {
            textCountdown.text = ((int)(countdown + 0.9f)).ToString();
        }
    }

    private void UnderGroundEmptyHeightInfo(Transform player)
    {
        underGroundEmptyHeight.text = "틈: " + Tools.GetInstance().GetUnderGroundEmptyHeight(player).ToString("F2");
    }

    public void PlayerRebirth()
    {
        deadUI.SetActive(false);
    }

    public void GamePause(bool pause)
    {
        pauseUI.SetActive(pause);
    }

    public void UnPauseAndStartMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("StartMenu");
    }

    private void AvailableStructuresInfo(int selectedStructureIndex)
    {
        for (int i = 0; i < availableStructureChild.Count; ++i)
        {
            Color newColor = (i == selectedStructureIndex) ? Color.white : Color.gray;
            availableStructureChild[i].GetComponent<Image>().color = newColor;
        }
    }

    private void BuiltStructuresInfo(List<GameObject> builtStructures)
    {
        for (int i = 0; i < builtStructures.Count; ++i)
        {
            builtStructureChild[i].GetComponent<Image>().sprite = Resources.Load("Images\\" +
                builtStructures[i].name + "Img"
                , typeof(Sprite)) as Sprite;
            
            builtStructureChild[i].transform.GetChild(0).GetComponent<Text>().text = EasyVector3(
                builtStructures[i].transform.position - Ground.GetInstance().groundPosition0
                );
        }
    }

    private void MinimumGroundHeightInfo()
    {
        groundMinimumHeight.text = "최저 높이: " + Ground.GetInstance().groundMinimumHeight.ToString();
    }

    private string EasyVector3(Vector3 vector3)
    {
        Vector3 switchYZ = new Vector3(
            vector3.x,
            vector3.z,
            vector3.y
            );

        return string.Format("{0:0},{1:0},{2:0}", switchYZ.x, switchYZ.y, switchYZ.z);
    }

    private string EasyFloat(float size)
    {
        return (decimal.Round((decimal)size, 1)).ToString();
    }

}
