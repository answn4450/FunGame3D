using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public GameObject shootHead;
    public GameObject bullet;
    public GameObject bulletPrefab;
    public bool fire;
    public bool act;

    private void Awake()
    {
        fire = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (act)
        {
            if (fire)
            {
                fire = false;
                bullet = Instantiate(PrefabManager.GetInstance().GetPrefabByName("Bullet"));
                bullet.transform.rotation = Quaternion.Euler(
                         0.0f,
                         transform.rotation.eulerAngles.y,
                         0.0f
                         );
                bullet.transform.position = shootHead.transform.position;
            }
        }

        if (!fire && !bullet)
            fire = true;
    }
}
