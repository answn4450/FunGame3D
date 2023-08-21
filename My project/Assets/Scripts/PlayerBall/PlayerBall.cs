using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : LivingBall, IPlayerBall
{
    protected string ballName;
    protected float deadSize;

    public string GetBallName()
    {
        return ballName;
    }

    public float GetDeadSize()
    {
        return deadSize;
    }
}
