using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject player;
    public float detectLength;
    public GameObject Top;
    public GameObject Bottom;
    public List<GameObject> shoots = new List<GameObject>();

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player)
        {
            if ((transform.position - player.transform.position).magnitude < detectLength)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, 5.0f, Time.deltaTime),
                    transform.position.z
                    );

                foreach (GameObject shoot in shoots)
                {
                    shoot.GetComponent<ShooterController>().act = true;
                    shoot.GetComponent<ShooterController>().FollowPoint(player.transform.position);
                }
            }
            else
            {
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, -5.0f, Time.deltaTime),
                    transform.position.z
                    );
            }
        }
    }
}
