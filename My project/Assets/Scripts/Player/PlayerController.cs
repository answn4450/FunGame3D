using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManualKey;

public class PlayerController : MonoBehaviour
{
    public CameraController playerCamera;

    public bool dead;
    public float size;

    private GameObject structPrefab;
    private GameObject effectFX;

    private float deadSize = 0.2f;
    private float gravityPower;
    private float horizontal;
    private float vertical;

    private float shotTimer;

    public bool rideBullet;
    private bool canDash;

    private Vector3 affectPower;


    public GameObject bullet;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        gravityPower = 9.8f;
        horizontal = 0.0f;
        vertical = 0.0f;

        shotTimer = 0.0f;

        affectPower = Vector3.zero;
        
        dead = false;
        canDash = true;
        rideBullet = false;
    }

    private void Start()
    {
        SetSphere(size);
        structPrefab = PrefabManager.GetInstance().GetPrefabByName("Jumper");
        transform.position = Vector3.zero;
        effectFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
    }
    
    void Update()
    {
        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

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

    public void Move()
    {
        Vector3 movement = Vector3.zero;
        float turnDeg = 0.0f;
        float turnSpeed;
        float speed;
        bool softTurn = (Input.GetKey(KeyCode.LeftShift));
        bool crabWalk;

        vertical *= 0.98f;
        horizontal *= 0.98f;

        if (Input.GetKey(KeyCode.DownArrow))
            vertical -= Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
            vertical += Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal -= Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            horizontal += Time.deltaTime;

        crabWalk = Input.GetKey(KeyCode.LeftControl);
        vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);
        horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);

        turnSpeed = softTurn ? 3.5f : 2.0f;
        speed = softTurn ? 10.0f : 7.0f;

        if (horizontal != 0)
        {
            if (crabWalk)
                transform.position += transform.right * horizontal * Time.deltaTime;
            else
            {
                turnDeg += turnSpeed * horizontal;
                playerCamera.SwivelZ(horizontal);
            }
        }

        if (vertical != 0)
        {
            movement += speed * vertical * transform.forward;
            playerCamera.ChangeFieldView(vertical);
        }

        transform.position += movement * 0.01f;
        transform.Rotate(Vector3.up * turnDeg);
    }

    public void Command()
	{
        if (Input.GetKeyDown((KeyCode)KeyboardQRow.BulletIsPlayer))
        {
            rideBullet = !rideBullet;
            if (rideBullet)
                OnRideBullet();
        }

        if (Input.GetKeyDown((KeyCode)KeyboardQRow.StructOnBullet))
            StructOnBullet();

        if (Input.GetKeyDown((KeyCode)KeyboardARow.Shot))
            Status.GetInstance().spaceKey = KeyboardARow.Shot;

        if (Input.GetKey(KeyCode.Space))
		{
            if (Status.GetInstance().spaceKey == KeyboardARow.Shot && shotTimer <= 0)
                Shot();
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
                SkillEffect(bullet.transform.position);
                Status.GetInstance().structureUse++;
            }
        }
    }

    private void SkillEffect(Vector3 position)
    {
        GameObject effect = Instantiate(effectFX);
        effect.transform.position = position;
    }

    private void Shot()
    {
        GameObject a = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(a);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
        shotTimer = 0.2f;
    }

    public void RideBullet()
    {
        if (bullet != null)
        {
            bullet.GetComponent<BulletController>().RideWithPlayer(gameObject);
            SetSphere(bullet.transform.localScale.x);
        }
        else
            rideBullet = false;
    }

    private void OnRideBullet()
    {
        if (bullet != null)
            SkillEffect(bullet.transform.position);
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

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private void CheckDead()
    {
        dead = transform.localScale.x <= deadSize;
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }

}
