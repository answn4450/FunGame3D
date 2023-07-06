using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject player;
    public float detectLength;
    public List<GameObject> shoots = new List<GameObject>();

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player)
        {
            if (
                (transform.position - player.transform.position).magnitude < detectLength
                )
                foreach (GameObject shoot in shoots)
                    shoot.GetComponent<ShooterController>().active = true;
                
        }
    }
}
