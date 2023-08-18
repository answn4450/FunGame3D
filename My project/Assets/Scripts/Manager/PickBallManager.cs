using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickBallManager : MonoBehaviour
{
    public Text TextdeadSize;
    public Text TextBallName;

    void Start()
    {
        TextdeadSize.text = "최소 내부: " + (2.0f).ToString();
        TextBallName.text = "평범한 공";
    }
}
