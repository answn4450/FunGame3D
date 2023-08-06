using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    private float height = 0.5f;

    public void Fall()
    {
        float fallY = Time.deltaTime;
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, Vector3.down, out hit, fallY + height))
            transform.position += Vector3.down * fallY;
    }

    public void OnGround()
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundMask))
            hit.transform.GetComponent<GroundController>().AddOnBoardCollider(gameObject);
    }
}