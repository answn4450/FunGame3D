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
    public float maxSize;

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
    private float rapidATimer;
    private float rapidDTimer;

    public bool rideBullet;

    private float gravity;
    private Vector3 gravityDirection;
    private Vector3 affectPower;

    public GameObject bullet;

    private void Awake()
    {
        size = 1.0f;
        maxSize = 1.0f;
        deadSize = 0.2f;

        horizontal = 0.0f;
        vertical = 0.0f;
        shotTimer = 0.0f;
        rapidATimer = 0.0f;
        rapidDTimer = 0.0f;

        physicsScale = 1.0f;
        physicsTimeElapseScale = 1.0f;
        inAirTime = 0.0f;

        affectPower = Vector3.zero;
        gravity = 9.8f;
        gravityDirection = Vector3.down;

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

    
    public void Hurt()
    {
        Hurt(0.1f);
    }

    public void Hurt(float hurt)
	{
        if (size > deadSize)
            size -= hurt;
        if (size < deadSize)
            size = deadSize;
    }

    public void Command()
    {
        if (Input.GetKeyDown((KeyCode)KeyboardQRow.BulletIsPlayer))
		{
            rideBullet = !rideBullet;
            if (rideBullet)
                OnRideBullet();
            else if (bullet)
                bullet.GetComponent<BulletController>().DestroySelf();
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

    public void Explode()
    {
        Instantiate(explodeFX, transform.position, transform.rotation);
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

    public void CommandMove()
    {
        Vector3 futureMove = GetCommandMovement() * Time.deltaTime;
        SafeMove(futureMove);
    }

    public void WithAffectPower()
    {
        transform.position += affectPower * Time.deltaTime * physicsTimeElapseScale;
        SafeMove(affectPower);
        affectPower *= Mathf.Clamp01(1 - Time.deltaTime * physicsTimeElapseScale);
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += CollideReflect(power * Time.deltaTime * physicsScale);
    }

    public void Rebirth()
	{
        dead = false;
        size = Mathf.Clamp(deadSize * 2.0f, deadSize + 0.1f, maxSize);
	}

    private void SafeMove(Vector3 move)
    {
        transform.position += move;

        float validX0 = Ground.GetInstance().groundX0 + size * 0.5f;
        float validX1 = Ground.GetInstance().groundX1 - size * 0.5f;
        float validZ0 = Ground.GetInstance().groundZ0 + size * 0.5f;
        float validZ1 = Ground.GetInstance().groundZ1 - size * 0.5f;
        float validY0 = Ground.GetInstance().groundY0 + 1.0f + size * 0.5f;
        float validY1 = Ground.GetInstance().groundY1 - size * 0.5f;
        
        if (transform.position.x < validX0 || transform.position.x > validX1)
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, validX0, validX1),
                transform.position.y,
                transform.position.z
                );

        if (transform.position.z < validZ0 || transform.position.z > validZ1)
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z, validZ0, validZ1)
                );

        if (transform.position.y < validY0 || transform.position.y > validY1)
        {
            affectPower = Vector3.Reflect(affectPower, Vector3.up);
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(transform.position.y, validY0, validY1),
                transform.position.z
                );
            playerCamera.PushXY(Vector2.up * size * affectPower.y * Time.deltaTime);
        }
    }

    private Vector3 GetCommandMovement()
    {
        Vector3 movement = Vector3.zero;
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

        speed = softTurn ? 13.0f : 10.0f;

        float rotateDeg = 20.0f;
        int dir = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            rapidATimer = 0.0f;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            rapidDTimer = 0.0f;
        
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
            rapidATimer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            rapidDTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow) && rapidATimer <= 0.0f)
		{
            rapidATimer = 0.2f;
            dir -= 1;
		}
        if (Input.GetKey(KeyCode.RightArrow) && rapidDTimer <= 0.0f)
		{
            rapidDTimer = 0.2f;
            dir += 1;
		}

        if (horizontal != 0)
        {
            if (crabWalk)
                movement += transform.right * horizontal * speed;
            else
            {
                transform.Rotate(transform.up * rotateDeg * dir);
                playerCamera.SwivelZ(horizontal);
            }
        }

        if (vertical != 0)
        {
            movement += speed * vertical * transform.forward;
            playerCamera.ChangeFieldView(vertical);
        }

        return movement;
    }

    private void StructOnBullet()
    {
        if (Status.GetInstance().structureUse < Status.GetInstance().structureMaxUse)
        {
            if (bullet != null)
            {
                GameObject _struct = Instantiate(structPrefab);
                _struct.transform.position = Ground.GetInstance().GetIndexPosition(transform.position);
                //SkillEffect(bullet.transform.position);
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
        GameObject _bullet = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(_bullet);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
        shotTimer = 0.2f;
    }

    private void OnRideBullet()
    {
        GameObject _bullet = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(_bullet);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
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

    private Vector3 CollideReflect(Vector3 inMove)
	{
        //inMove += inMove.normalized * size * 0.5f;
        Vector3 outDirection = inMove.normalized;
        float outDistance = inMove.magnitude * 0.05f;
        int onGroundIndexX = Tools.GetInstance().GetGroundIndexX(transform.position.x);
        int onGroundIndexZ = Tools.GetInstance().GetGroundIndexZ(transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, inMove, out hit, inMove.magnitude, groundMask))
        {
            int collideGroundIndexX = Tools.GetInstance().GetGroundIndexX(hit.transform.position.x);
            int collideGroundIndexZ = Tools.GetInstance().GetGroundIndexZ(hit.transform.position.z);
            //if (onGroundIndexX == collideGroundIndexX && onGroundIndexZ == collideGroundIndexZ)
                return outDirection * outDistance;
        }
        else
            return outDirection * outDistance;
	}

    private void GroundGravityCollision()
    {
        if (InAir())
            inAirTime += Time.deltaTime;
        else
            inAirTime = 0.0f;

        AffectPower(gravityDirection * gravity * inAirTime * inAirTime * size);
    }

    private bool InAir()
    {
        return !Physics.Raycast(transform.position, gravityDirection, size * 0.5f);
    }
}
