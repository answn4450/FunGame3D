using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public CameraController playerCamera;

    public bool dead;
    public float size;

    public GameObject turnPoint;

    public string structPositionName;
    public string structTypeName;
    public string attackTypeName;

    public int useType;

    private GameObject structPrefab;
    private float deadSize = 0.2f;
    private float gravityPower;
    private Vector3 affectPower;


    private GameObject bullet;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        dead = false;
        gravityPower = 9.8f;
        affectPower = Vector3.zero;
        structPositionName = "플레이어 앞";
        structTypeName = "점퍼";
        attackTypeName = "일반 총알";

        useType = 0;
    }

    private void Start()
    {
        SetSphere(size);
        structPrefab = PrefabManager.GetInstance().GetPrefabByName("Jumper");
        transform.position = Vector3.zero;
    }
    
    void Update()
    {
        CheckDead();
        SphereBySize(size);
    }

    public void WithAffectPower()
    {
        if (transform.position.y > size * 0.5f)
            Fall();
        transform.position += affectPower * Time.deltaTime;
        affectPower *= 1 - Time.deltaTime;
        GroundCollision();
    }

    public void Explode()
    {
        //dead = true;
    }

    public void Hurt()
    {
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += power * Time.deltaTime;
    }

    public void Command()
	{
        if (Input.GetKeyDown(KeyCode.Q))
            useType = 0;
        if (Input.GetKeyDown(KeyCode.W))
            useType = 1;
        if (Input.GetKeyDown(KeyCode.E))
            useType = 2;

        if (Input.GetKeyDown(KeyCode.Space))
		{
            if (useType == 0)
			{
                
			}
            else if (useType == 1)
			{
                if (Status.GetInstance().structureUse < Status.GetInstance().structureMaxUse)
                {
                    GameObject _struct = Instantiate(structPrefab);
                    _struct.transform.position = transform.position;
                    Status.GetInstance().structureUse++;
                }
            }
            else if (useType == 2)
			{
                Shot();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
		{
            if (useType == 2)
                PlayerIsBullet();
		}
    }


    private void Shot()
    {
        GameObject a = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(a);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
        //bullets.Add(bullet);
    }

    private void PlayerIsBullet()
    {
        if (bullet != null)
        {
            Vector3 pos = bullet.transform.position;
            bullet.transform.position = transform.position;
            transform.position = pos;

            GameObject effect = Instantiate(
                PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof")
            );
            effect.transform.position = transform.position;
        }
    }

    private void Fall()
    {
        if (gravityPower < 9.8f + 6.0f)
            gravityPower += Time.deltaTime;
        if (gravityPower > 9.8f + 6.0f)
            gravityPower = 9.8f + 6.0f;

        transform.position += Vector3.down * gravityPower * gravityPower;
    }

    private void GroundCollision()
    {
        float groundWidthHalf = Status.GetInstance().groundWidth * 0.5f;
        float groundHeightHalf = Status.GetInstance().groundHeight * 0.5f;

        if (transform.position.x < -groundWidthHalf || transform.position.x > groundWidthHalf)
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -groundWidthHalf, groundWidthHalf),
                transform.position.y,
                transform.position.z
                );

        if (transform.position.x < -groundHeightHalf || transform.position.x > groundHeightHalf)
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.x, -groundHeightHalf, groundHeightHalf)
                );

        if (transform.position.y < size * 0.5f)
            playerCamera.PushXY(Vector2.down * size * affectPower.y * 0.5f);
            playerCamera.PushXY(Vector2.down * size * affectPower.y * 0.5f);

        if (transform.position.y <= size * 0.5f)
        {
            gravityPower = 0.0f;
            transform.position = new Vector3(
                transform.position.x,
                size*0.5f,
                transform.position.z
                );
            affectPower -= Vector3.down * affectPower.y;
        }
    }

    public void Move()
    {
        Vector3 movement = Vector3.zero;
        float turnDeg = 0.0f;
        float turnSpeed;
        float speed;

        bool hardTurn = (Input.GetKey(KeyCode.LeftShift));
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        turnSpeed = hardTurn ? 140.0f : 90.0f;
        speed = hardTurn ? 5.0f : 10.0f;

        if (horizontal != 0)
		{
            turnDeg += turnSpeed * horizontal;
            playerCamera.SwivelZ(horizontal);
		}

        if (vertical != 0)
		{
            movement += speed * vertical * transform.forward;
            playerCamera.ChangeFieldView(vertical);
		}

        transform.position += movement * 0.01f;
        transform.Rotate(new Vector3(0.0f, turnDeg, 0.0f) * Time.deltaTime);
    }

    private void SphereBySize(float size)
	{
        float newSize = Mathf.Lerp(
            transform.localScale.x,
            size,
            Time.deltaTime
            );

        if (Mathf.Abs(size - newSize) < 0.01)
            newSize = size;

        SetSphere(newSize);
	}

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }

    private void CheckDead()
    {
        dead = transform.localScale.x <= deadSize;
    }

    private void OnTriggerEnter(Collider other)
    {
    }

}
