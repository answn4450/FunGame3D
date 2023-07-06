using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Vector3 dir;

    void Start()
    {
        GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
        a.transform.position = transform.position;
        
        dir = transform.rotation.eulerAngles.normalized;
        dir = gameObject.transform.forward;
        Debug.Log(dir);

        a.transform.position += dir * 3;
    }
}
