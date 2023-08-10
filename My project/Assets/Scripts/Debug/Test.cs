using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Test : MonoBehaviour
{
    public Transform a1;
    public Transform a2;
    public Transform axis1;
    public Transform axis2;
    public Transform b1;
    public Transform b2;

	public List<Renderer> objectRenderers = new List<Renderer>();
	private const string path = "Legacy Shaders/Transparent/Specular";

    private void Update()
    {
        RaycastHit hit;
        Vector3 move = Vector3.down;
        float radius = 0.5f;
        Debug.DrawLine(transform.position, transform.position + (Vector3.down + Vector3.right) * radius);
        if (Physics.Raycast(transform.position, Vector3.down + Vector3.right, out hit, radius))
            Debug.Log(hit.transform.name);
        else
            Debug.Log("not detected");
    }
    /*
	private void Update()
    {
		TransparentBlocks();
        //test();
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
    */
}
