using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CameraController playerCamera;
    public PlayerEyeController playerEye;

    [System.NonSerialized]
    public Transform structureFolder;
    
    public bool dead;
    public bool rideBullet;
    public float size;
    public float maxSize;

    public LayerMask groundMask;

    public GameObject bullet;

    private GameObject effectFX;
    private GameObject explodeFX;
    private GameObject hurtFX;

    private List<string> availableStructures = new List<string> {
        "Jumper",
        "StopAura",
    };

    private int selectedStructureIndex;
    private float deadSize = 0.2f;
    private float horizontal;
    private float vertical;
    private float physicsScale;
    private float physicsTimeElapseScale;
    private float shotTimer;
    private float inAirTime;
    private float rapidATimer;
    private float rapidDTimer;

    private float gravity;

    private bool stopFall;
    private const int maxStructure = 3;
    private List<GameObject> builtStructures = new List<GameObject>();

    private Vector3 gravityDirection;
    private Vector3 affectPower;


    private void Awake()
    {
        structureFolder = null;
        
        dead = false;
        rideBullet = false;
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

        stopFall = false;
        selectedStructureIndex = (int)(availableStructures.Count * 0.5f);
    }

    private void Start()
    {
        SetSphere(size);
        transform.position = Vector3.zero;
        effectFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        explodeFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        hurtFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
    }

    void Update()
    {
        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

        Fall();
        CheckDead();
        SphereBySize(size);
        playerEye.FollowTarget(transform);
    }

    public List<string> GetAvailableStructures()
    {
        return availableStructures;
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
        if (Input.GetKeyDown(KeyCode.A))
            StructOnBullet();

        if (Input.GetKeyDown(KeyCode.S))
		{
            rideBullet = !rideBullet;
            if (rideBullet)
                OnRideBullet();
            else if (bullet)
                OffRideBullet();
		}

        if (Input.GetKey(KeyCode.Space))
        {
            if (shotTimer <= 0)
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
            OffRideBullet();
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
        affectPower += power;
    }

    public void Rebirth()
	{
        dead = false;
        size = Mathf.Clamp(deadSize * 2.0f, deadSize + 0.1f, maxSize);
	}

    public void ChangeSelectedStructureIndex()
    {
        int size = availableStructures.Count;
        for (int i = 0; i < size; ++i)
        {
            if (Input.GetKey(KeyCode.Alpha1 + i))
                selectedStructureIndex = i;
        }
    }

    private void SafeMove(Vector3 move)
    {
        transform.position += move;

        float radius = transform.localScale.x * 0.5f;
        float validX0 = Ground.GetInstance().groundX0 + radius * 0.5f;
        float validX1 = Ground.GetInstance().groundX1 - radius * 0.5f;
        float validZ0 = Ground.GetInstance().groundZ0 + radius * 0.5f;
        float validZ1 = Ground.GetInstance().groundZ1 - radius * 0.5f;
        float validY0 = Ground.GetInstance().groundY0 + 1.0f + radius * 0.5f;
        float validY1 = Ground.GetInstance().groundY1 - radius * 0.5f;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );
        /*
        if (transform.position.x - radius < validX0 || transform.position.x > validX1)
        {
            ApplyReflect(
                new Vector3(
                    Mathf.Clamp(transform.position.x, validX0, validX1),
                    transform.position.y,
                    transform.position.z
                ),
                Vector3.right
                );
        }

        if (transform.position.y - radius < validY0 || transform.position.y > validY1)
        {
            ApplyReflect(
                new Vector3(
                    transform.position.x,
                    Mathf.Clamp(transform.position.y, validY0, validY1),
                    transform.position.z
                ),
                Vector3.up
                );
        }

        if (transform.position.z - radius < validZ0 || transform.position.z > validZ1)
        {
            ApplyReflect(
                new Vector3(
                    transform.position.x,
                    transform.position.y,
                    Mathf.Clamp(transform.position.z, validZ0, validZ1)
                ),
                Vector3.forward
                );
        }
     
        RaycastHit hit;
        bool block = false;
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        Vector3 collisionPoint = transform.position;
        if (vertices.Length != 0)
        {
            foreach (Vector3 point in vertices)
            {
                if (block = Physics.Raycast(transform.position, move, out hit, moveDistance))
                    collisionPoint = hit.transform.position;
            }
            if (!block)
                ApplyReflect(hit.transform.position, Vector3.right);
        }
        */
    }

    public int GetSelectedStructureIndex()
    {
        return selectedStructureIndex;
    }

    public List<GameObject> GetBuiltStructures()
    {
        return builtStructures;
    }

    public int GetMaxStructure()
    {
        return maxStructure;
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
        if (builtStructures.Count < maxStructure)
        {
            if (bullet != null)
            {
                string name = availableStructures[selectedStructureIndex];
                GameObject _struct = Instantiate(PrefabManager.GetInstance().GetPrefabByName(name), structureFolder);
                _struct.name = name;
                _struct.transform.position = Tools.GetInstance().GetGroundIndexPosition(bullet.transform.position);
                SkillEffect(bullet.transform.position);
                builtStructures.Add(_struct);
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
        stopFall = true;
        GameObject _bullet = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(_bullet);
        bullet.GetComponent<BulletController>().BirthBullet(gameObject);
        SkillEffect(bullet.transform.position);
    }

    private void OffRideBullet()
    {
        stopFall = false;
        if (bullet)
            bullet.GetComponent<BulletController>().DestroySelf();
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

    private void Fall()
    {
        if (InAir() && !stopFall)
            inAirTime += Time.deltaTime;
        else
            inAirTime = 0.0f;

        AffectPower(gravityDirection * gravity * inAirTime * inAirTime * size);
    }

    private bool InAir()
    {
        return !Physics.Raycast(transform.position, gravityDirection, size * 0.5f);
    }

    private void ApplyReflect(Vector3 collisionPoint, Vector3 inNormal)
    {
        Vector3 inDirection = collisionPoint - transform.position;
        float inDistance = inDirection.magnitude;
        Vector3 outDirection = Vector3.Reflect(collisionPoint, inNormal);

        transform.position = collisionPoint + outDirection * inDistance * 0.5f;
        affectPower = outDirection * inDistance * 0.5f;
    }
}
