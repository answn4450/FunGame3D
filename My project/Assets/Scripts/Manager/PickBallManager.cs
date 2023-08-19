using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickBallManager : MonoBehaviour
{
    public Text textDeadSize;
    public Text textBallName;
    public Transform validBallNamesPannel;
    public GameObject validBallModelPlate;
    public IPlayerBall pIckInfoBall;
    private List<string> validBallNames;

    private void Awake()
    {
        validBallNames = new List<string>();
    }

    void Start()
    {
        pIckInfoBall = GameObject.Find("ModelNormalBall").GetComponent<NormalBall>();
        textBallName.text = pIckInfoBall.GetBallName();
        textDeadSize.text = "최소 내부: " + pIckInfoBall.GetDeadSize().ToString();

        for (int i = 0; i < validBallModelPlate.transform.childCount; ++i)
        {
            IPlayerBall ball = validBallModelPlate.transform.GetChild(i).GetComponent<IPlayerBall>();
            validBallNames.Add(ball.GetBallName());
        }

        Transform validBallTextSample = validBallNamesPannel.GetChild(0);
        validBallTextSample.gameObject.SetActive(false);
        foreach (string ballName in validBallNames)
        {
            Button candidateBallName = Instantiate(validBallTextSample.gameObject).GetComponent<Button>();
            candidateBallName.gameObject.SetActive(true);
            candidateBallName.transform.SetParent(validBallNamesPannel);
            candidateBallName.transform.GetChild(0).GetComponent<Text>().text = ballName;
        }
    }

    public void PressChangePick(Text newPickBallName)
    {
        string pickBallName = newPickBallName.text;
    }
}
