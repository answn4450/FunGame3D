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

    public LayerMask groundMask;

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

    private float gravity;
    private Vector3 gravityDirection;
    private Vector3 affectPower;
    private Vector3 movement;
    private List<Vector3> checkDirection;

    public GameObject bullet;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        horizontal = 0.0f;
        vertical = 0.0f;

        physicsScale = 0.1f;
        physicsTimeElapseScale = 10.0f;
        shotTimer = 0.0f;
        inAirTime = 0.0f;

        affectPower = Vector3.zero;
        gravity = 9.8f;
        gravityDirection = Vector3.down;
        checkDirection = new List<Vector3>();
        checkDirection.Add(Vector3.left);
        checkDirection.Add(Vector3.right);
        checkDirection.Add(Vector3.up);
        checkDirection.Add(Vector3.down);

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
        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

        GroundGravityCollision();
        CheckDead();
        SphereBySize(size);
        playerEye.FollowTarget(transform);
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

    public void Hurt()
    {
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;
    }

    public void Move()
    {
        Vector3 movement = GetMovement();
        SafeMove(movement);
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
        affectPower *= Mathf.Clamp01(1 - Time.deltaTime * physicsTimeElapseScale);
        GroundPannelCollision();
    }

    public void BindPosition()
    {
        
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += power * Time.deltaTime * physicsScale;
    }

    public void Explode()
    {
        Instantiate(explodeFX, transform.position, transform.rotation);
    }

    private bool FallCheck()
    {
        return false;
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

    private void SafeMove(Vector3 movement)
    {
        Vector3 dir = movement * Time.deltaTime;
        transform.position += dir;
    }

    private Vector3 GetMovement()
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
                movement += transform.right * horizontal * speed;
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

        transform.Rotate(Vector3.up * turnDeg);

        return movement;
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

    private void CheckDead()
    {
        bool before = dead;
        dead = size <= deadSize;
        if (dead && !before)
            Explode();
    }

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }

    private void GroundPannelCollision()
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

    private void GroundGravityCollision()
    {
        if (InAir())
            inAirTime += Time.deltaTime;
        else
            inAirTime = 0.0f;

        AffectPower(gravityDirection * gravity * inAirTime * inAirTime);
    }

    private bool InAir()
    {
        return !Physics.Raycast(transform.position, gravityDirection, size * 0.5f);
    }
}
