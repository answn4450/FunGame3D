using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public ElevatorController elevator;

    void Start()
    {
        elevator.enableChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (elevator && elevator.IsWithPlayer())
            elevator.MovePlayer();
    }
}
