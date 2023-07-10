using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool dead;
    public float size;
    private float deadSize = 0.2f;
    private Vector3 jumpPower;
    private Vector3 boostPower;
    private Vector3 lastPower;

    private void Awake()
    {
        size = 1.0f;
        deadSize = 0.2f;
        dead = false;
        boostPower = new Vector3(0.0f, 5.0f, 0.0f);
        lastPower = Vector3.zero;
        jumpPower = boostPower;
    }

    private void Start()
    {
        SetSphere(size);
    }

    void Update()
    {
        if (!dead)
            Move();

        SphereBySize(size);

        if (CheckDead())
            Explode();

    }

    private void Move()
    {
        Vector3 movement = Vector3.zero;
        float turnDeg = 0.0f;
        bool hardTurn;
        float turnSpeed;
        float speed;
        hardTurn = (Input.GetKey(KeyCode.LeftShift));
        turnSpeed = hardTurn ? 140.0f : 90.0f;
        speed = hardTurn ? 5.0f : 10.0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            turnDeg -= turnSpeed;

        if (Input.GetKey(KeyCode.RightArrow))
            turnDeg += turnSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
		{
            if (Input.GetKey(KeyCode.UpArrow))
                transform.position += boostPower * Time.deltaTime;
            if (Input.GetKey(KeyCode.DownArrow))
                transform.position -= boostPower * Time.deltaTime;
        }
        else
		{
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movement -= speed * transform.forward;
                lastPower = movement/2;
                jumpPower = -boostPower;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement += speed * transform.forward;
                lastPower = movement/2;
                jumpPower = boostPower;
            }
        }

        lastPower = lastPower.normalized * Mathf.Lerp(
            lastPower.magnitude, 0.0f, Time.deltaTime*2
            );

        transform.position += movement * Time.deltaTime;
        Debug.Log(lastPower.magnitude);
        transform.position += lastPower * Time.deltaTime;
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

    private bool CheckDead()
    {
        return transform.localScale.x <= deadSize;
    }
}
