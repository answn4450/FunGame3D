using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    //hello, ����Ű, turn, turn �������� turn, (�ĸ±�), dash(�� ������), ������ ����->��.
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
        requireString = "Enter -> �ѱ��";
        bigInfo.GetNewText("�ȳ� Ʃ�丮��");
    }

    void Update()
    {
        if (step==0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bigInfo.GetNewText("����Ű ����");
                requireString = "Enter -> �ѱ��";
                step += 1;
            }
        }
        if (step == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
                bigInfo.GetNewText("����Ű ����");
            
            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            Debug.Log(distance);
            if (distance < movePoint.transform.localScale.x)
            {
                bigInfo.GetNewText("��ġ");
                requireString = "<- Ű, ";
                //step++;
            }
        }
        else if (step == 1)
        {

        }

        requireInfo.text = requireString;
    }
}
