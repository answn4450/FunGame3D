using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : Structure
{
    public GameObject outPortal;


    private void OnTriggerEnter(Collider other)
    {
        if (outPortal)
            other.transform.position = outPortal.transform.position;
    }
}
