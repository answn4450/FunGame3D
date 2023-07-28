using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StructureCollision;

public class Test : MonoBehaviour
{
    public Transform a1;
    public Transform a2;
    public Transform axis1;
    public Transform axis2;
    public Transform b1;
    public Transform b2;

	private void Update()
    {
        test();
            //a1.position = Vector3.Reflect(a2.position, Vector3.right);
    }

    private void test()
	{
        Vector3 dir = a2.position - a1.position;
        Vector3 axis = axis2.position - axis1.position;
        Vector3 inNormal = Quaternion.Euler(0.0f, 0.0f, 90.0f) * axis;
        Vector3 dir2 = Vector3.Reflect(dir.normalized, inNormal.normalized);
        b2.position = b1.position + dir2 * 3;
        //b2.position = b1.position + inNormal.normalized * 2;
    }
}
