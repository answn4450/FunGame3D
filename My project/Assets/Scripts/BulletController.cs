using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector]
    private float speed;
    private bool stop;

    private void Awake()
    {
        speed = 10.0f;
        stop = false;
    }

    void Update()
    {
        if (!stop)
            transform.position += transform.forward * speed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().Hurt();
            //stop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
