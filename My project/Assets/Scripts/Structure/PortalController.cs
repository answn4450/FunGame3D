using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public GameObject outPortal;


    private void OnTriggerEnter(Collider other)
    {
        if (outPortal)
            other.transform.position = outPortal.transform.position;
    }
}
