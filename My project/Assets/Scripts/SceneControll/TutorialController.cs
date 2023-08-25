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
    public List<string> ment;

    public int step;
    private Sandbag whiteBall;
    private string nextElevatorName;
    private GameObject nextElevator;
    private GameObject signFX;

    private void Awake()
    {
        step = 0;
        ment = new List<string> {
            "0>안녕 튜토리얼",
            "1>화살표 키로 이동/회전",
            "2>하얀 공으로 이동>하얀 공에 터치",
            "3>목적지 도착!",
            "4>스페이스바로 하얀 공에 총알 날리기>스페이스바=총알 발사",
            "5>turn point 조작>Enter로 넘기기",
            "6>ctrl+위쪽/아래쪽 화살표로 간격 조정>ctrl+위쪽/아래쪽 화살표",
            "7>이제 ctrl + 왼쪽/오른쪽 화살표로 회전>ctrl + 왼쪽/오른쪽 화살표 키",
            "8>현재 선택 건축물은 1번.",
            "9>숫자키2로 후보 건축물 2번으로 변경>2번 건축물 StopAura",
            "10>a로 건축물을 발사한 총알 위치에 생성>A로 총알이 없어지기 전에 StopAura 생성",
            "11>s로 빠른 이동 시작 및 해제>S로 총알 변신",
            "12>엘리베이터로 탑승으로 튜토리얼 종료>파란색 엘리베이터",
        };

        whiteBall = GameObject.Find("whiteBall").GetComponent<Sandbag>();
        signFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Fire");
    }

    void Start()
    {
        NewPage(step);
        nextElevatorName = "nextElevator";
        nextElevator = GameObject.Find(nextElevatorName);
        nextElevator.SetActive(false);
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
                if (Input.GetKey(KeyCode.LeftControl) && Mathf.Abs(Input.GetAxis("Vertical")) >= 1.0f)
                    nextStep = true;
                break;
            case 7:
                if (Input.GetKey(KeyCode.LeftControl) && Mathf.Abs(Input.GetAxis("Horizontal")) >= 1.0f)
                    nextStep = true;
                break;
            case 8:
                if (Input.GetKeyDown(KeyCode.Return))
                    nextStep = true;
                break;
            case 9:
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    nextStep = true;
                break;
            case 10:
                if (GameObject.Find("StopAura"))
                    nextStep = true;
                break;
            case 11:
                if (Input.GetKeyDown(KeyCode.S))
                    nextStep = true;
                break;
            case 12:
                if (!GameObject.Find(nextElevatorName))
                {
                    nextElevator.SetActive(true);
                    Instantiate(signFX).transform.position = nextElevator.transform.position;
                }
                break;
        }

        if (nextStep)
        {
            NewPage(++step);
        }
    }

    private void NewPage(int page)
    {
        string big, small;
        string[] splits = ment[page].Split('>');
        big = splits[1];
        small = splits.Length > 2 ? splits[2] : "Enter로 넘기기";
        bigInfo.GetNewText(big);
        requireInfo.text = small;
    }

}
