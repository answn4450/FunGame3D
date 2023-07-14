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

    private float deadSize = 0.2f;
    private float gravityPower;

    private float turnLength = 2.0f;
    private Vector3 affectPower;
    private float rushTime;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        dead = false;
        gravityPower = 9.8f;
        rushTime = 4.0f;
        affectPower = Vector3.zero;
    }

    private void Start()
    {
        SetSphere(size);
    }
    
    void Update()
    {
        CheckDead();
        SphereBySize(size);

        if (!OnGround())
            Fall();

        if (dead)
            Explode();
        else
		{
            Move();
            transform.position += affectPower;
            if (Input.GetKeyUp(KeyCode.Space))
                StartCoroutine(Rush());
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
        gravityPower +=  Time.deltaTime;
        transform.position -= Vector3.up * gravityPower * gravityPower * Time.deltaTime;
        if (OnGround())
        {
            gravityPower = 9.8f;
            playerCamera.PushXY(Vector2.down * size*10);
        }
    }

    private bool OnGround()
    {
        return transform.position.y <= size * 0.5;
    }

    public void BindPosition(float groundX0, float groundX1)
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

        if (transform.position.y < size * 0.5)
        {
            transform.position = new Vector3(
                transform.position.x,
                size * 0.5f,
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
            turnDeg += turnSpeed * horizontal;

        if (vertical != 0)
            movement += speed * vertical * transform.up;

        transform.position += movement * 0.01f;
        Debug.Log(transform.forward);
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
