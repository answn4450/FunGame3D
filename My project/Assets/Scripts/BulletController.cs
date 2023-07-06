using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector]
    private Vector3 direction;
    private float speed;

    private void Awake()
    {
        direction = new Vector3(1.0f, 0.0f, 0.0f);
        speed = 2.0f;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnCollisionExit(Collision collision)
    {
        //Destroy(gameObject);
        //Debug.Log("a");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("b");
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        //Debug.Log("c");
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("d");
    }
}
