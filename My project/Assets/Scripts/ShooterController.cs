using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public GameObject shootHead;
    public GameObject bullet;
    public GameObject bulletPrefab;
    public bool fire;
    public bool active;

    private void Awake()
    {
        fire = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (active)
        {
            if (fire)
            {
                fire = false;
                bullet = Instantiate(PrefabManager.GetInstance().GetPrefabByName("Bullet"));
                bullet.transform.position = shootHead.transform.position;
            }
        }

        if (!fire && !bullet)
            fire = true;
    }
}
