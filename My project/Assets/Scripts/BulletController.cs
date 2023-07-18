using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector]
    private float speed;
    private bool stop;
    private string parentName;
    private bool released;

    private void Awake()
    {
        speed = 10.0f;
        stop = false;
        released = false;
    }

    void Update()
    {
        if (!stop)
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void BirthBullet(GameObject parent)
    {
        transform.forward = parent.transform.forward;
        transform.position = parent.transform.position;
        parentName = parent.transform.name;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (released)
        {
            Destroy(gameObject);
            if (other.tag == "Player")
            {
                other.GetComponent<PlayerController>().Hurt();
                //stop = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!released && other.transform.name == parentName)
            released = true;
        else if (released)
            Destroy(gameObject);

        StartCoroutine(KnockBack(other.gameObject, 1));
    }

    IEnumerator KnockBack(GameObject hit, float t)
    {
        while (t>0.0f && hit != null)
        {
            yield return null;
            hit.transform.position += transform.forward * t * Time.deltaTime;
            t -= Time.deltaTime;
        }
    }
}
