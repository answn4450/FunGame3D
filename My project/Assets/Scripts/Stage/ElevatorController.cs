using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public int destStage;
    private bool withPlayer;
    public bool arrive;

    void Start()
    {
        arrive = true;
    }

    private void LateUpdate()
    {
        withPlayer = false;
    }

    private bool OnTriggerExit(Collider other)
    {
        return true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            withPlayer = true;
    }

    public bool WithPlayer()
    {
        return withPlayer;
    }

    public void MoveUpDown(int dir)
    {

    }

}
