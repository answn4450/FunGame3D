using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManualKey;

public class PlayerController : MonoBehaviour
{
    public CameraController playerCamera;
    public PlayerEyeController playerEye;

    public bool dead;
    public float size;

    private GameObject structPrefab;
    private GameObject effectFX;
    private GameObject explodeFX;
    private GameObject hurtFX;

    private float deadSize = 0.2f;
    private float horizontal;
    private float vertical;
    private float physicsScale;
    private float physicsTimeElapseScale;
    private float shotTimer;
    private float inAirTime;

    public bool rideBullet;

    private Vector3 gravity;
    private Vector3 affectPower;

    public GameObject bullet;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        horizontal = 0.0f;
        vertical = 0.0f;

        physicsScale = 0.02f;
        physicsTimeElapseScale = 2.0f;
        shotTimer = 0.0f;
        inAirTime = 0.0f;

        affectPower = Vector3.zero;
        gravity = 9.8f * Vector3.down;

        dead = false;
        rideBullet = false;

    }

    private void Start()
    {
        SetSphere(size);
        structPrefab = PrefabManager.GetInstance().GetPrefabByName("Jumper");
        transform.position = Vector3.zero;
        effectFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        explodeFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        hurtFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        StartCoroutine(EyeControll());
    }
    
    void Update()
    {
        if (transform.position.y > size * 0.5f + Status.GetInstance().groundY)
            inAirTime += Time.deltaTime;
        else
            inAirTime = 0.0f;

        AffectPower(gravity * inAirTime * inAirTime);

        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

        CheckDead();
        SphereBySize(size);
        playerEye.FollowTarget(transform);
    }

    public void Hurt()
    {
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;
    }
    
    IEnumerator EyeControll()
	{
        float rotateDeg = 30;
        int dir;
        bool rapidA, rapidD;
        while (true)
		{
            dir = 0;
            rapidA = Input.GetKey(KeyCode.A) && !Input.GetKeyDown(KeyCode.A);
            rapidD = Input.GetKey(KeyCode.D) && !Input.GetKeyDown(KeyCode.D);
            
            if (rapidA || rapidD)
                yield return new WaitForSeconds(0.2f);
            else
                yield return null;

            if (Input.GetKey(KeyCode.A))
                dir += 1;
            if (Input.GetKey(KeyCode.D))
                dir -= 1;
            
            transform.Rotate(transform.up * rotateDeg * dir);
        }
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
        speed = softTurn ? 13.0f : 10.0f;

        if (horizontal != 0)
        {
            if (crabWalk)
                transform.position += transform.right * horizontal * speed * Time.deltaTime;
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

        transform.position += movement * Time.deltaTime;
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

    public void WithAffectPower()
    {
        transform.position += affectPower * Time.deltaTime * physicsTimeElapseScale;
        affectPower *= (1 - Time.deltaTime * physicsTimeElapseScale);
        GroundCollision();
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += power * Time.deltaTime * physicsScale;
    }

    public void Explode()
    {
        Instantiate(explodeFX, transform.position, transform.rotation);
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

    /*
    private void CollisionReflect()
	{
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, Mathf.Infinity))
            affectPower = Vector3.Reflect(affectPower, Vector3.forward);
        if (Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity))
            affectPower = Vector3.Reflect(affectPower, Vector3.forward);
        if (Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity))
            affectPower = Vector3.Reflect(affectPower, Vector3.forward);
        if (Physics.Raycast(transform.position, -transform.right, out hit, Mathf.Infinity))
            affectPower = Vector3.Reflect(affectPower, Vector3.forward);
    }
    */

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
            playerCamera.PushXY(Vector2.down * size * affectPower.y * Time.deltaTime);

        if (transform.position.y <= groundY)
        {
            affectPower = Vector3.Reflect(affectPower, Vector3.up);
            transform.position = new Vector3(
                transform.position.x,
                groundY,
                transform.position.z
                );
        }

    }

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private void CheckDead()
    {
        bool before = dead;
        dead = size <= deadSize;
        if (dead && !before)
            Explode();
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }
}
