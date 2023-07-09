using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool dead;
    private int durability;
    private float size = 1;
    private float deadSize = 0.2f;
    private float Health = 10.0f;
    private Vector3 TurnAxis;

    private void Awake()
    {
        durability = 5;
        dead = false;
        TurnAxis = new Vector3(0.0f, 1.0f, 0.0f);
        transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;
        float turnDeg = 0.0f;
        bool hardTurn;
        float turnSpeed;
        float speed;
        hardTurn = (Input.GetKey(KeyCode.LeftShift));
        turnSpeed = hardTurn ? 140.0f : 90.0f;
        speed = hardTurn ? 5.0f : 10.0f;
        
        if (Input.GetKey(KeyCode.DownArrow))
		{
            movement -= speed * transform.forward;
            print("sadf");
		}

        if (Input.GetKey(KeyCode.UpArrow))
            movement += speed * transform.forward;

        
        if (Input.GetKey(KeyCode.LeftArrow))
            turnDeg -= turnSpeed;

        if (Input.GetKey(KeyCode.RightArrow))
            turnDeg += turnSpeed;

        transform.position += movement * Time.deltaTime;

        transform.Rotate(new Vector3(0.0f, turnDeg, 0.0f) * Time.deltaTime);
    }

    public void KnockBack(float _damage)
    {
        Health -= _damage;
    }

    private void SetSize(float size)
	{
        transform.localScale = new Vector3(size, size, size);
	}

    void ChangeSize(float newSize)
	{
	}

    public void Explode()
	{
        dead = true;
	}

    public void Hurt()
	{
        durability -= 1;
        Explode();
	}
}
