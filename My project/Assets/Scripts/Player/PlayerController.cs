using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManualKey;

public class PlayerController : MonoBehaviour
{

    public CameraController playerCamera;

    public bool dead;
    public float size;

    public GameObject turnPoint;

    public string structPositionName;
    public string structTypeName;
    public string attackTypeName;

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
        affectPower *= (1 - Time.deltaTime);
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
        if (Input.GetKey((KeyCode)KeyboardQRow.BulletIsPlayer))
            Status.GetInstance().qRowKey = KeyboardQRow.BulletIsPlayer;
        else if (Input.GetKey((KeyCode)KeyboardQRow.StructOnBullet))
            Status.GetInstance().qRowKey = KeyboardQRow.StructOnBullet;
        else if (Input.GetKey((KeyCode)KeyboardQRow.Blank))
            Status.GetInstance().qRowKey = KeyboardQRow.Blank;

        if (Input.GetKey((KeyCode)KeyboardARow.Shot))
            Status.GetInstance().aRowKey = KeyboardARow.Shot;
        else if (Input.GetKey((KeyCode)KeyboardARow.Dash))
            Status.GetInstance().aRowKey = KeyboardARow.Dash;
        else if (Input.GetKey((KeyCode)KeyboardARow.Blank))
            Status.GetInstance().aRowKey = KeyboardARow.Blank;

        if (Input.GetKeyDown(KeyCode.Space))
		{
            SpaceQRow();
        }
    }

    private void SpaceQRow()
    {
        switch (Status.GetInstance().qRowKey)
        {
            case KeyboardQRow.StructOnBullet:
                StructOnBullet();
                break;
            case KeyboardQRow.BulletIsPlayer:
                PlayerIsBullet();
                break;
            case KeyboardQRow.Blank:
                SpaceARow();
                break;
        }
    }

    private void SpaceARow()
    {
        switch (Status.GetInstance().aRowKey)
        {
            case KeyboardARow.Shot:
                Shot();
                break;
            case KeyboardARow.Dash:
                Dash();
                break;
        }
    }

    private void StructOnBullet()
    {
        if (Status.GetInstance().structureUse < Status.GetInstance().structureMaxUse)
        {
            if (bullet != null)
            {
                GameObject _struct = Instantiate(structPrefab);
                _struct.transform.position = bullet.transform.position;
                Status.GetInstance().structureUse++;
            }
        }
    }

    private void Dash()
    {
        Debug.Log("Dash");
    }

    private void Shot()
    {
        GameObject a = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(a);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
    }

    private void PlayerIsBullet()
    {
        if (bullet != null)
        {
            //bullet.GetComponent<BulletController>().TransportPlayer(gameObject);
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
        float groundX0 = Status.GetInstance().groundX0 + size * 0.5f;
        float groundX1 = Status.GetInstance().groundX1 - size * 0.5f;
        float groundZ0 = Status.GetInstance().groundZ0 + size * 0.5f;
        float groundZ1 = Status.GetInstance().groundZ1 - size * 0.5f;
        float groundY = Status.GetInstance().groundY + size * 0.5f;

        if (transform.position.x < groundX0 || transform.position.x > groundX1)
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, groundX0, groundX1),
                transform.position.y,
                transform.position.z
                );

        if (transform.position.z < groundZ0 || transform.position.z > groundZ1)
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z, groundZ0, groundZ1)
                );

        if (transform.position.y < groundY)
            playerCamera.PushXY(Vector2.down * size * affectPower.y * 0.5f);

        if (transform.position.y <= groundY)
        {
            gravityPower = 0.0f;
            transform.position = new Vector3(
                transform.position.x,
                groundY,
                transform.position.z
                );
            AffectPower(
                Mathf.Min(affectPower.y - Time.deltaTime, 0.9f) * Vector3.down
                );
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
