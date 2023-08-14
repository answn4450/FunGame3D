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
    public List<string> ment;

    private Sandbag whiteBall;

    private void Awake()
    {
        step = 0;
        ment = new List<string> {
            "0>안녕 튜토리얼>Enter로 넘기기",
            "1>화살표 키로 앞뒤양옆으로 이동>방향키 조작 후 Enter",
            "2>목적지로 이동>하얀 공 수색",
            "3>목적지 도착!>Enter로 넘기기",
            "4>스페이스바로 하얀 공에 총알 날리기>스페이스바=총알 발사",
            "5>현재 선택 건축물은 1번.>Enter로 넘기기",
            "6>숫자키2로 후보 건축물 2번으로 변경>2번 건축물 StopAura",
            "7>a로 건축물을 발사한 총알 위치에 생성>A로 총알이 없어지기 전에 StopAura 생성",
            "8>s로 빠른 이동 시작 및 해제>S로 총알 변신",
            "9>엘리베이터로 탑승으로 튜토리얼 종료>파란색 엘리베이터",
        };

        whiteBall = GameObject.Find("whiteBall").GetComponent<Sandbag>();
    }

    void Start()
    {
        requireInfo.text = "Enter -> 넘기기";
        bigInfo.GetNewText("안녕 튜토리얼");
    }

    void Update()
    {
        bool nextStep = false;
        switch (step)
        {
            case 0:
                if (Input.GetKeyDown(KeyCode.Return))
                    nextStep = true;
                break;
            case 1:
                if (Input.GetKeyDown(KeyCode.Return))
                    nextStep = true;
                break;
            case 2:
                float distance = (player.transform.position - whiteBall.transform.position).magnitude;
                if (distance * 2 <= player.transform.localScale.x + whiteBall.transform.localScale.x)
                    nextStep = true;
                break;
            case 3:
                if (Input.GetKeyDown(KeyCode.Return))
                    nextStep = true;
                break;
            case 4:
                if (whiteBall.satisfy)
                    nextStep = true;
                break;
            case 5:
                if (Input.GetKeyDown(KeyCode.Return))
                    nextStep = true;
                break;
            case 6:
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    nextStep = true;
                break;
            case 7:
                if (GameObject.Find("StopAura"))
                    nextStep = true;
                break;
            case 8:
                if (Input.GetKeyDown(KeyCode.S))
                    nextStep = true;
                break;
        }

        if (nextStep)
        {
            step++;
            NewPage();
        }
    }

    private void NewPage()
    {
        string big, small;
        string[] splits = ment[step].Split('>');
        big = splits[1];
        small = splits.Length > 2 ? splits[2] : "";
        bigInfo.GetNewText(big);
        requireInfo.text = small;
    }

}
