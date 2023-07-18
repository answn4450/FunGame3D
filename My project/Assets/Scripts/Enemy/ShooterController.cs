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

    private float minLocalY;
    private float maxLocalY;
    private float worldY;

    private void Awake()
    {
        fire = true;
        minLocalY = -1.0f;
        maxLocalY = 1.0f;
        worldY = 0.0f;
    }

    void Start()
    {
        BindLocalY();
    }

    void Update()
    {
        if (act)
        {
            if (fire && worldY > 0.0f)
            {
                fire = false;
                bullet = Instantiate(PrefabManager.GetInstance().GetPrefabByName("Bullet"));
                bullet.GetComponent<BulletController>().BirthBullet(shootHead);
            }
        }

        if (!fire && !bullet)
            fire = true;

    }

	private void LateUpdate()
	{
        SetPosition();
    }

	public void FollowPoint(Vector3 point)
    {
        float t = Mathf.Clamp01(Time.deltaTime);

        worldY = Mathf.Lerp(worldY, point.y, t);

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

    private void SetPosition()
	{
        transform.position = new Vector3(
            transform.position.x,
            worldY,
            transform.position.z
            );

        if (BindLocalY() != transform.localPosition.y)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                BindLocalY(),
                transform.localPosition.z
                );
        }
    }

    private float BindLocalY()
    {
        float bindY = Mathf.Clamp(transform.localPosition.y, minLocalY, maxLocalY);
            
        return bindY;
    }
}
