using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    //hello, ����Ű, turn, turn �������� turn, (�ĸ±�), dash(�� ������), ������ ����->��.
    public BigInfoController bigInfo;
    public Text requireInfo;
    public GameObject player;
    public GameObject movePoint;
    public int step;

    private void Awake()
    {
        step = 0;
    }

    void Start()
    {
        requireInfo.text = "Enter -> �ѱ��";
        bigInfo.GetNewText("�ȳ� Ʃ�丮��");
    }

    void Update()
    {
        if (step==0)
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
                bigInfo.GetNewText("����Ű ����");
            
            float distance = Vector3.Distance(player.transform.position, movePoint.transform.position);
            Debug.Log(distance);
            if (distance < movePoint.transform.localScale.x)
            {
                bigInfo.GetNewText("��ġ");
                requireInfo.text =  "<- Ű, ";
                //step++;
            }
        }
        if (step==2)
		{
            if (Input.GetKeyDown(KeyCode.Return))
			{
                bigInfo.GetNewText("Ʃ�丮�� �����ϰ� ���� ����");
                requireInfo.text =  "Enter -> �ѱ��";
                step++;
			}
        }
        if (step == 3)
		{
            if (Input.GetKeyDown(KeyCode.Return))
                SceneManager.LoadScene("Play");
        }
    }
}
