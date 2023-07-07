using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float Health = 10.0f;
    private float Speed;
    private Vector3 TurnAxis;

    private void Awake()
    {
        TurnAxis = new Vector3(0.0f, 1.0f, 0.0f);
        Speed = 10.0f;
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;
        float turnDeg = 0.0f;
        bool axel;
        float turnSpeed;

        if (Input.GetKey(KeyCode.DownArrow))
            movement -= Speed * transform.forward;

        if (Input.GetKey(KeyCode.UpArrow))
            movement += Speed * transform.forward;

        axel = (Input.GetKey(KeyCode.LeftShift));
        turnSpeed = axel ? 140.0f : 90.0f;
        
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
}
