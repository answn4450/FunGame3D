using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public BigInfoController bigInfo;
    public Text requireInfo;
    public GameObject player;
    public GameObject movePoint;
    [SerializeField]
    [Range(0,7)]
    public int step;

    private void Awake()
    {
        //step = 0;
    }

    void Start()
    {
        requireInfo.text = "Enter -> 넘기기";
        bigInfo.GetNewText("안녕 튜토리얼");
    }

    void Update()
    {
        if (step < 4)
            StepMove0to3();
        else if (step < 7)
            StepControl4to6();
        else
            StepEnd7();
    }

    private void StepMove0to3()
    {
        if (step == 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bigInfo.GetNewText("방향키 조작");
                requireInfo.text = "Enter -> 넘기기";
                step += 1;
            }
        }
        if (step == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bigInfo.GetNewText("목적지로 이동");
                requireInfo.text = "하얀 공 수색";
                step += 1;
            }
        }
        if (step == 2)
        {

            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            if (distance < movePoint.transform.localScale.x)
            {
                step = 3;
                bigInfo.GetNewText("목적지 도착!");
                requireInfo.text = "Enter -> 넘기기";
            }
        }
        if (step == 3)
        {
            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            if (distance > movePoint.transform.localScale.x)
            {
                bigInfo.GetNewText("목적지로 이동");
                requireInfo.text = "하얀 공 수색";
                step = 2;
            }

            if (Input.GetKeyDown(KeyCode.Return))
                step += 1;
        }
    }

    private void StepControl4to6()
    {
        if (step == 4)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                bigInfo.GetNewText("공격");
                requireInfo.text = "Space -> 총알 공격";
                step += 1;
            }
        }
        if (step == 5)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                bigInfo.GetNewText("총알과 위치 교환");
                requireInfo.text = "Tab -> 위치 변환";
                step = 6;
            }
        }
        if (step == 6)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                bigInfo.GetNewText("튜토리얼 완료!");
                requireInfo.text = "Space -> 게임 시작";
                step += 1;
            }
        }
    }

    private void StepEnd7()
    {
        if (step == 7)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                SceneManager.LoadScene("Play");
            }
        }
    }
}
