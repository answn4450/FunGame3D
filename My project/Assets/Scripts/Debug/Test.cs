using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StructureCollision;

public class Test : MonoBehaviour
{
    /*
    public LayerMask mask;
    public GameObject dot;
    private Vector3 direction;
    private float distance = 3.0f;
    
    private void test(Transform coll)
    {
        RaycastHit hit;

        Physics.Raycast(transform.position, direction, out hit, distance, mask);
        Debug.DrawRay(transform.position, direction, Color.yellow);
        dot.transform.position = hit.transform.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        BackCollision backCollision = new BackCollision();
        backCollision.Back(transform, other.gameObject);
    }
    */

    private bool space = false;

    private void Start()
    {
        StartCoroutine(test());
    }

    private void Update()
    {
        if (space)
            Debug.Log(Time.deltaTime);
    }

    IEnumerator test()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.Space))
        {
            space = true;
            yield return new WaitForSeconds(0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(test());
        }
    }
}
