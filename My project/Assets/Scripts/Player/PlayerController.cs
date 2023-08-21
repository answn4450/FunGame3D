using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NormalBall
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
    public GameObject turnPoint;

    private const int maxStructure = 3;
    private int selectedStructureIndex;
    //private float physicsScale;
    private const float turnPointMaxLength = 5.0f;
    private const float turnPointMinLength = 1.0f;
    private const float hoverSpeed = 7.0f;
    private float physicsTimeElapseScale;
    private float shotTimer;
    private float inFallTime;
    private float turnPointLength;
    private bool stopFall;
    private bool hoverTurnPoint;

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

    private List<GameObject> builtStructures = new List<GameObject>();

    private Vector3 gravity;
    private Vector3 affectPower;


    private void Awake()
    {
        structureFolder = null;

        dead = false;
        rideBullet = false;
        hoverTurnPoint = false;

        size = 1.0f;
        maxSize = 1.0f;

        shotTimer = 0.0f;
        turnPointLength = 2.0f;
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

        matBlue = Resources.Load("Material/BasicColor/BasicBlue", typeof(Material)) as Material;
        matRed = Resources.Load("Material/BasicColor/BasicRed", typeof(Material)) as Material;
        transform.GetComponent<Renderer>().material = matBlue;
    }

    private void Start()
    {
        SetSphere(size);
    }

    public void LifeCycle()
    {
        if (shotTimer > 0.0f)
            shotTimer -= Time.deltaTime;

        Fall();
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

        if (Input.GetKey(KeyCode.Space) && shotTimer <= 0)
            Shot();

        hoverTurnPoint = Input.GetKey(KeyCode.LeftControl);
        if (hoverTurnPoint)
            TurnPointSameY();
        else
            TurnPointOnForward();
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
        float speed = hardTurn ? 6.0f : 10.0f;
        float horizontal, vertical;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            horizontal = Input.GetAxis("Horizontal");
        else
            horizontal = 0.0f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            vertical = Input.GetAxis("Vertical");
        else
            vertical = 0.0f;

        if (hoverTurnPoint)
            HoverTurnPoint(horizontal, vertical);
        else if (vertical != 0)
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
        float rotateDeg = hardTurn ? 110.0f : 50.0f;

        if (!hoverTurnPoint)
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

    public void Squeeze(float requiredDown)
    {
        squeezeFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        Instantiate(squeezeFX).transform.position = transform.position;

        Hurt(requiredDown);
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
        BindGroundStandY();
    }

    public void FallInfo()
    {
        Debug.LogFormat("InAir: {0}, stopFall: {1}, fallTime: {2}", InAir(), stopFall, inFallTime);
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

    private void BindGroundStandY()
    {
        GroundController underGround = Tools.GetInstance().GetUnderGround(transform);
        float radius = transform.localScale.x * 0.5f;
        float groundTop = Tools.GetInstance().GetTopY(underGround.transform);

        if (transform.position.y - radius < groundTop)
            transform.position = new Vector3(
                transform.position.x,
                groundTop + radius,
                transform.position.z
                );

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
        float outSize = transform.localScale.x;
        dead = size <= deadSize && outSize <= deadSize;
        if (dead && !before)
            Explode();
    }

    private void SetSphere(float r)
    {
        transform.localScale = Tools.GetInstance().GetSphereVector3(r);
    }

    private void Fall()
    {
        if (InAir() && !stopFall)
            inFallTime += Time.deltaTime;
        else
            inFallTime = 0.0f;

        AffectPower(gravity * inFallTime * inFallTime);
    }

    private void EasyCheckColor()
    {
        if (stopFall)
            GetComponent<Renderer>().material = matRed;
        else
            GetComponent<Renderer>().material = matBlue;

        if (hoverTurnPoint)
            turnPoint.GetComponent<Renderer>().material = matRed;
        else
            turnPoint.GetComponent<Renderer>().material = matBlue;

    }

    private void HoverTurnPoint(float horizontal, float vertical)
    {
        ChangeTurnPointLength(vertical);
        
        /*
        Vector3 movement = Vector3.zero;
        
        Vector3 playerToTurnPoint = Tools.GetInstance().GetDirectionXZ(
            transform, turnPoint.transform);
        float deg = Mathf.Atan2(playerToTurnPoint.x, playerToTurnPoint.z) * Mathf.Rad2Deg;
        float diameter = Mathf.PI * turnPointLength * 2.0f;
        float nextDeg = deg + 360.0f * hoverSpeed / diameter * horizontal;
        Vector3 nextPos = turnPoint.transform.position + turnPointLength * new Vector3(
            Mathf.Cos(nextDeg * Mathf.Deg2Rad),
            turnPoint.transform.position.y,
            Mathf.Sin(nextDeg * Mathf.Deg2Rad)
            );
        movement += Time.deltaTime * (nextPos - transform.position);
        
        SafeMove(movement);
        */
    }

    private void ChangeTurnPointLength(float vertical)
    {
        Vector3 movement = Vector3.zero;
        float lengthDiff = -vertical * hoverSpeed * Time.deltaTime;

        if (turnPointLength + lengthDiff > turnPointMaxLength)
            lengthDiff = turnPointMaxLength - turnPointLength;
        if (turnPointLength + lengthDiff < turnPointMinLength)
            lengthDiff = turnPointMinLength - turnPointLength;

        movement += -GetTurnPointXZDirection().normalized * lengthDiff;
        SafeMove(movement);

        turnPointLength = Mathf.Clamp(
            GetTurnPointXZDirection().magnitude,
            turnPointMinLength,
            turnPointMaxLength
            );
    }

    private void TurnPointOnForward()
    {
        turnPoint.transform.position = transform.position + turnPointLength * transform.forward;
    }

    private void TurnPointSameY()
    {
        float diffY = transform.position.y - turnPoint.transform.position.y;
        turnPoint.transform.position += diffY * Vector3.up;
    }

    private Vector3 GetTurnPointXZDirection()
    {
        return Tools.GetInstance().GetDirectionXZ(turnPoint.transform, transform);
    }
}