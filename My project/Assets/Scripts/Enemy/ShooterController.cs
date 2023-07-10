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
            if (fire && transform.position.y > 0.0f)
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

    public void FollowPoint(Vector3 point)
    {
        float t = Mathf.Clamp01(Time.deltaTime);
        float y = Mathf.Lerp(transform.position.y, point.y, t);
        float beforeY = transform.position.y;

        transform.position = new Vector3(
            transform.position.x,
            y,
            transform.position.z
            );

        if (transform.localPosition.y > 1 || transform.localPosition.y < -1)
            transform.position = new Vector3(
                transform.position.x,
                beforeY,
                transform.position.z
                );

        float rotateDeg = (
            Mathf.Rad2Deg * -Mathf.Atan2(
                point.z - transform.position.z,
                point.x - transform.position.x
                )
            + 360.0f + 90.0f) % 360.0f;

        float shooterDeg = (transform.rotation.eulerAngles.y + 360.0f) % 360.0f;

        if (Mathf.Abs(rotateDeg - shooterDeg) > 180.0f)
        {
            if (rotateDeg < shooterDeg)
                rotateDeg += 360.0f;
            if (shooterDeg < rotateDeg)
                shooterDeg += 360.0f;
        }

        transform.rotation = Quaternion.Euler(
            0.0f,
            Mathf.Lerp(shooterDeg, rotateDeg, t),
            0.0f
            );
    }
}
