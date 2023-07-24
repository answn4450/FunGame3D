using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private bool withPlayer;

    private void Awake()
    {
        withPlayer = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
            withPlayer = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            withPlayer = true;
    }

    public bool IsWithPlayer()
    {
        return withPlayer;
    }

}
