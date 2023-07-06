using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float Health = 10.0f;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.DownArrow))
            movement -= transform.forward;

        if (Input.GetKey(KeyCode.UpArrow))
            movement += transform.forward;

        transform.position += movement * Time.deltaTime;
    }

    public void KnockBack(float _damage)
    {
        Health -= _damage;
    }
}
