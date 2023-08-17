using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingBall
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
    private GameObject squeezeFX;
    private Material matBlue;
    private Material matRed;

    private List<string> availableStructures = new List<string> {
        "Jumper",
        "StopAura",
    };

    private int selectedStructureIndex;
    private float deadSize = 0.2f;
    //private float physicsScale;
    private float physicsTimeElapseScale;
    private float shotTimer;
    private float inFallTime;
    private bool stopFall;
    private const int maxStructure = 3;
    private List<GameObject> builtStructures = new List<GameObject>();

    private Vector3 gravity;
    private Vector3 affectPower;


    private void Awake()
    {
        structureFolder = null;
        
        dead = false;
        rideBullet = false;
        size = 1.0f;
        maxSize = 1.0f;
        deadSize = 0.2f;

        shotTimer = 0.0f;

        physicsTimeElapseScale = 1.0f;
        inFallTime = 0.0f;

        affectPower = Vector3.zero;
        gravity = 9.8f * Vector3.down;

        stopFall = false;
        selectedStructureIndex = (int)(availableStructures.Count * 0.5f - 0.1f);

        effectFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        explodeFX = PrefabManager.GetInstance().GetPrefabByName("CFX_MagicPoof");
        hurtFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        squeezeFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");

        matBlue = Resources.Load("Material/BasicBlue", typeof(Material)) as Material;
        matRed = Resources.Load("Material/BasicRed", typeof(Material)) as Material;
        transform.GetComponent<Renderer>().material = matBlue;
    }

    private void Start()
    {
        SetSphere(size);
        //transform.position = Vector3.zero;
    }

    public void LifeCycle()
    {
        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

        //Fall();
        CheckDead();
        playerEye.FollowTarget(transform);
        EasyCheckColor();
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
            else
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

    public void CommandMoveBody()
    {
        Vector3 movement = Vector3.zero;
        bool hardTurn = (Input.GetKey(KeyCode.LeftShift));
        bool crabWalk = Input.GetKey(KeyCode.LeftControl);
        float speed = hardTurn ? 7.0f : 10.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (!(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)))
            vertical = 0.0f;
        if (!(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
            horizontal = 0.0f;

        if (crabWalk)
            movement += transform.right * horizontal * speed;

        if (vertical != 0)
        {
            movement += speed * vertical * transform.forward;
            playerCamera.ChangeFieldView(vertical);
        }

        SafeMove(movement * Time.deltaTime);
    }

    public void CommandTurnEye()
    {
        int dir = 0;
        bool hardTurn = Input.GetKey(KeyCode.LeftShift);
        bool crabWalk = Input.GetKey(KeyCode.LeftControl);
        float rotateDeg = hardTurn ? 90.0f : 50.0f;
        
        if (!crabWalk)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                dir -= 1;
            if (Input.GetKey(KeyCode.RightArrow))
                dir += 1;

            transform.Rotate(transform.up * rotateDeg * dir * Time.deltaTime);
            playerCamera.SwivelZ(dir);
        }
        
    }

    public void WithAffectPower()
    {
        SafeMove(affectPower * Time.deltaTime * physicsTimeElapseScale);
        affectPower *= Mathf.Clamp01(1 - Time.deltaTime * physicsTimeElapseScale);
    }

    public void AffectPower(Vector3 power)
    {
        affectPower += power;
    }

    public float Squeeze(float requiredDown)
    {
        squeezeFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        Instantiate(squeezeFX).transform.position = transform.position;

        float localSize = transform.localScale.x;
        float lifeMile = localSize - deadSize;
        float downSize = lifeMile * requiredDown * Time.deltaTime;
        SetSphere(localSize - downSize);

        return downSize;
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

    public void BackToSize()
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

    public int GetSelectedStructureIndex()
    {
        return selectedStructureIndex;
    }

    public int GetMaxStructure()
    {
        return maxStructure;
    }

    public bool IsSizeBigger()
    {
        return size > transform.localScale.x;
    }

    public List<GameObject> GetBuiltStructures()
    {
        return builtStructures;
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
        rideBullet = false;
        stopFall = false;
        if (bullet)
            bullet.GetComponent<BulletController>().DestroySelf();
        else
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, Ground.GetInstance().groundX0, Ground.GetInstance().groundX1),
                Mathf.Clamp(transform.position.y, Ground.GetInstance().groundY0, Ground.GetInstance().groundY1),
                Mathf.Clamp(transform.position.z, Ground.GetInstance().groundZ0, Ground.GetInstance().groundZ1)
                );
        }
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
            inFallTime += Time.deltaTime;
        else
            inFallTime = 0.0f;
        
        //Debug.LogFormat("InAir: {0}, stopFall: {1}, fallTime: {2}", InAir(), stopFall, inFallTime);
        AffectPower(gravity * inFallTime * inFallTime);
    }

    private void EasyCheckColor()
    {
        if (stopFall)
            GetComponent<Renderer>().material = matRed;
        else
            GetComponent<Renderer>().material = matBlue;
    }

}
