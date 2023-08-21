using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBase : MonoBehaviour
{
    protected string asdf;

    public void Update()
    {
        if (transform.GetComponent<TestA>())
        {
            //gameObject.AddComponent<TestB>
            Destroy(transform.GetComponent<TestA>());
        }
    }
}
