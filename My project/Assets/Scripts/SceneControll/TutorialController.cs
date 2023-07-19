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
        requireInfo.text = "Enter -> �ѱ��";
        bigInfo.GetNewText("�ȳ� Ʃ�丮��");
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
                bigInfo.GetNewText("����Ű ����");
                requireInfo.text = "Enter -> �ѱ��";
                step += 1;
            }
        }
        if (step == 1)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                bigInfo.GetNewText("�������� �̵�");
                requireInfo.text = "�Ͼ� �� ����";
                step += 1;
            }
        }
        if (step == 2)
        {

            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            if (distance < movePoint.transform.localScale.x)
            {
                step = 3;
                bigInfo.GetNewText("������ ����!");
                requireInfo.text = "Enter -> �ѱ��";
            }
        }
        if (step == 3)
        {
            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            if (distance > movePoint.transform.localScale.x)
            {
                bigInfo.GetNewText("�������� �̵�");
                requireInfo.text = "�Ͼ� �� ����";
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
                bigInfo.GetNewText("����");
                requireInfo.text = "Space -> �Ѿ� ����";
                step += 1;
            }
        }
        if (step == 5)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                bigInfo.GetNewText("�Ѿ˰� ��ġ ��ȯ");
                requireInfo.text = "Tab -> ��ġ ��ȯ";
                step = 6;
            }
        }
        if (step == 6)
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                bigInfo.GetNewText("Ʃ�丮�� �Ϸ�!");
                requireInfo.text = "Space -> ���� ����";
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
