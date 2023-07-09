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

                    float t = Mathf.Clamp01(Time.deltaTime);
                    shoot.transform.position = new Vector3(
                        shoot.transform.position.x,
                            Mathf.Clamp(
                                Mathf.Lerp(shoot.transform.position.y, player.transform.position.y, t),
                                Bottom.transform.position.y,
                                Top.transform.position.y
                            ),
                        shoot.transform.position.z
                        );

                    float rotateDeg = (
                        Mathf.Rad2Deg * -Mathf.Atan2(
                            player.transform.position.z - shoot.transform.position.z,
                            player.transform.position.x - shoot.transform.position.x
                            ) 
                        + 360.0f + 90.0f) % 360.0f;

                    float shooterDeg = (shoot.transform.rotation.eulerAngles.y + 360.0f) % 360.0f;

                    if (Mathf.Abs(rotateDeg - shooterDeg) > 180.0f)
                    {
                        if (rotateDeg < shooterDeg)
                            rotateDeg += 360.0f;
                        if (shooterDeg < rotateDeg)
                            shooterDeg += 360.0f;
                    }
                    
                    shoot.transform.rotation = Quaternion.Euler(
                        0.0f,
                        Mathf.Lerp(shooterDeg, rotateDeg,t),
                        0.0f
                        );
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
