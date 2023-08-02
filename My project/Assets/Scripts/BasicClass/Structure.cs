using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public void Fall()
    {
        float fallY = Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.transform.name);
            float limitFallY = transform.position.y - hit.transform.position.y - 0.5f;
            if (limitFallY < Time.deltaTime)
                fallY = Mathf.Max(limitFallY, fallY);
        }

        transform.position += Vector3.down * fallY;
    }
}