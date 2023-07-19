using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public CameraController playerCamera;
    public GameObject playerPoint;

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

    private float turnLength = 2.0f;
    private Vector3 affectPower;
    private Vector3 structPosition;
    private float rushTime;

    private float groundX0, groundX1, groundY;

    private GameObject bullet;
    private List<GameObject> bullets;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        dead = false;
        gravityPower = 9.8f;
        rushTime = 4.0f;
        affectPower = Vector3.zero;

        groundX0 = -4.0f;
        groundX1 = 4.0f;
        groundY = 0.0f;

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
        Command();
        CheckDead();
        SphereBySize(size);

        if (dead)
            Explode();
        else
        {
            Move();
            transform.position += affectPower * Time.deltaTime;
            if (transform.position.y > groundY + size * 0.5f)
            {
                Fall();
            }
            collisionGround();

            BindPosition();
        }
    }

    private void Command()
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
                GameObject _struct = Instantiate(structPrefab);
                _struct.transform.position = transform.position;
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


    private void UpdateBullets()
    {
        
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
        //CFX3_Hit_SmokePuff
        //CFX_MagicPoof
        //CFX4 Hit B (Orange)
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

    IEnumerator Rush()
	{
        while (rushTime >= 0.1f)
		{
            yield return null;
            
            //transform.position += Vector3.RotateTowards(transform.forward, transform.eulerAngles.y * transform.forward, 0.2f,6.2f) * Time.deltaTime;
            rushTime -= Time.deltaTime;
		}
        rushTime = 2.0f;
	}

    private void Fall()
    {
        if (gravityPower < 9.8f + 6.0f)
            gravityPower += Time.deltaTime;
        if (gravityPower > 9.8f + 6.0f)
            gravityPower = 9.8f + 6.0f;

        transform.position += Vector3.down * gravityPower * gravityPower;
    }

    private void collisionGround()
	{
        if (transform.position.y < groundY + size * 0.5f)
            playerCamera.PushXY(Vector2.down * size * affectPower.y * 0.5f);
        if (transform.position.y <= groundY + size * 0.5f)
        {
            gravityPower = 0.0f;
            affectPower = new Vector3(
                affectPower.x,
                0.0f,
                affectPower.z
                );
        }

    }

    private void BindPosition()
    {
        if (transform.position.x < groundX0)
            transform.position = new Vector3(
                groundX0, 
                transform.position.y, 
                transform.position.z);

        if (transform.position.x > groundX1)
            transform.position = new Vector3(
                groundX1,
                transform.position.y,
                transform.position.z);

        if (transform.position.y < groundY + size * 0.5)
        {
            transform.position = new Vector3(
                transform.position.x,
                groundY + size * 0.5f,
                transform.position.z
                );
        }
    }

    private void Move()
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

    private void TurnByPoint()
    {
        if (turnLength > 1.0f)
            turnLength -= Time.deltaTime;

        float deg = Vector3.RotateTowards(transform.position, turnPoint.transform.position, 0.0f, Time.deltaTime).y;
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

    public void Explode()
	{
        dead = true;
	}

    public void Hurt()
	{
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;
	}

    private void CheckDead()
    {
        dead = transform.localScale.x <= deadSize;
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += power * Time.deltaTime;
    }
}
