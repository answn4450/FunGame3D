using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GameObject bullet;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            test();
    }

    public void test()
    {
        GameObject a = PrefabManager.GetInstance().GetPrefabByName("Bullet");
        bullet = Instantiate(a);
        bullet.transform.position = transform.position;
        bullet.transform.forward = transform.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
    }

    private void FxTest()
    {

    }
}
