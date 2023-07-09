using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector]
    private float speed;

    private void Awake()
    {
        speed = 10.0f;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionExit(Collision collision)
    {
        Destroy(gameObject);
        //Debug.Log("a");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("b");
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        if (other.tag == "Player")
            other.GetComponent<PlayerController>().Hurt();
        //Debug.Log("c");
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
        //Debug.Log("d");
    }
}
