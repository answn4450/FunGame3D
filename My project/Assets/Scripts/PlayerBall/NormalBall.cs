using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBall : PlayerBall
{
    private void Awake()
    {
        ballName = "Normal Ball";
        deadSize = 0.2f;
    }
}
