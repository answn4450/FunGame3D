using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CameraController playerCamera;
    public GameObject playerPoint;

    public bool dead;
    public float size;

    private float deadSize = 0.2f;
    private float rushPower;
    private bool unAffect;
    private float gravityPower;
    private int bulletCount;

    private float groundY = 0.0f;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        dead = false;
        unAffect = false;
        gravityPower = 9.8f;
        bulletCount = 1;
    }

    private void Start()
    {
        SetSphere(size);
    }

    void Update()
    {
        CheckDead();
        SphereBySize(size);

        if (!CollideGround())
            Fall();

        if (dead)
            Explode();
        else
            Move();

        BindPosition();
    }

    private void Fall()
    {
        gravityPower +=  Time.deltaTime;
        transform.position -= Vector3.up * gravityPower * gravityPower * Time.deltaTime;
        if (CollideGround())
        {
            gravityPower = 9.8f;
            playerCamera.PushXY(Vector2.down * size*10);
        }
    }

    private bool CollideGround()
    {
        return transform.position.y <= size * 0.5;
    }

    private void BindPosition()
    {
        if (transform.position.y < size * 0.5)
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
            turnDeg += turnSpeed * horizontal;

        if (vertical != 0)
            movement += speed * vertical * transform.forward;

        transform.position += movement * Time.deltaTime;
        transform.Rotate(new Vector3(0.0f, turnDeg, 0.0f) * Time.deltaTime);
    }

    private void turnByPoint(float angle)
    {
        //transform.RotateAround
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
        transform.position += power * Time.deltaTime;
    }
}
