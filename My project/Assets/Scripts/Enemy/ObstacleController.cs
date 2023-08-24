using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject player;
    public float detectLength;
    public GameObject Top;
    public GameObject Bottom;
    public ShooterController shoot;

    private float standUpY = -4.0f;
    private float standDownY = -6.0f;

    void Start()
    {
        player = GameObject.Find("Player");
    }

	void Update()
    {
        if (player)
        {
            float destY;
            if ((transform.position - player.transform.position).magnitude < detectLength)
            {
                destY = standUpY;
                shoot.act = true;
                shoot.FollowPoint(player.transform.position);
            }
            else
            {
                destY = standDownY;
            }
            /*
            transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, destY, Time.deltaTime),
                    transform.position.z
                    );
            */
        }
    }
}
