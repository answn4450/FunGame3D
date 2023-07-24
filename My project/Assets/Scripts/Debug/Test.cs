using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject a= GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.AddComponent<Rigidbody>();
            a.GetComponent<Rigidbody>().useGravity = false;
            
        }
    }
}
