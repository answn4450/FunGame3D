using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    //hello, 방향키, turn, turn 설정으로 turn, (쳐맞기), dash(적 때리기), 아이템 지형->끝.
    public BigInfoController bigInfo;
    public Text requireInfo;
    public GameObject player;
    public GameObject movePoint;
    public int step;
    private string requireString;

    private void Awake()
    {
        step = 0;
    }

    void Start()
    {
        requireString = "Enter -> 넘기기";
        bigInfo.GetNewText("안녕 튜토리얼");
    }

    void Update()
    {
        if (step==0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bigInfo.GetNewText("방향키 조작");
                requireString = "Enter -> 넘기기";
                step += 1;
            }
        }
        if (step == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                bigInfo.GetNewText("방향키 조작");
            
            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            Debug.Log(distance);
            if (distance < movePoint.transform.localScale.x)
            {
                bigInfo.GetNewText("터치");
                requireString = "<- 키, ";
                //step++;
            }
        }
        else if (step == 1)
        {

        }

        requireInfo.text = requireString;
    }
}
