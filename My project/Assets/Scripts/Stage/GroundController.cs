using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;
    private float speed;
    private float distCheck;

    private void Awake()
    {
        speed = 0.1f;
        distCheck = 2.0f;
    }

    public void UpDownByTarget(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        SetSize(distCheck - distVector.magnitude);
        //SetSize(distVector.magnitude);
    }

    public void SqueezeGround()
    {
        SetSize(-1);
    }

    private void SetSize(float plusMinus)
    {
        if (transform.localScale.y + 0.5f + plusMinus > maxSizeY)
            plusMinus = maxSizeY - transform.localScale.y - 0.5f;
        if (transform.localScale.y + plusMinus < 1.0f)
            plusMinus = 1.0f -transform.localScale.y;

        Vector3 movement = Vector3.up * plusMinus * speed;
        transform.localScale += movement * Time.deltaTime;
        transform.position += movement * 0.5f * Time.deltaTime;
    }
}
