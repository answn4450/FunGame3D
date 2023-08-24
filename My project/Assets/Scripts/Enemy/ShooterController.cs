using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    public GameObject shootHead;
    public GameObject bullet;
    private GameObject bulletPrefab;
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
        bulletPrefab = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        BindLocalY();
    }

    void Update()
    {
        if (act)
        {
            if (fire && worldY > 0.0f)
            {
                fire = false;
                bullet = Instantiate(bulletPrefab);
                bullet.GetComponent<BulletController>().BirthBullet(shootHead, true);
            }
        }

        if (!fire && !bullet)
            fire = true;

    }

	void LateUpdate()
	{
        SetPosition();
    }

	public void FollowPoint(Vector3 point)
    {
        float t = Mathf.Clamp01(Time.deltaTime);

        worldY = Mathf.Lerp(worldY, point.y, t);

        float shooterDeg = transform.rotation.eulerAngles.y;
        float rotateDeg = 90.0f - Tools.GetInstance().GetZXAtan2(transform.position, point);

        transform.rotation = Quaternion.Euler(
            0.0f,
            shooterDeg + Mathf.DeltaAngle(shooterDeg, rotateDeg) * t,
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
