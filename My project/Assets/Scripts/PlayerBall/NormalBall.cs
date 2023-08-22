using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBall : PlayerBall, IPlayerBall
{
    private const string _ballName = "Normal Ball";
    private const float _deadSize = 0.2f;
    private const float _startBallSize = 1.0f;
    private const float _maxBallSize = 1.2f;

    public string GetBallName()
    {
        return _ballName;
    }

    public float GetDeadSize()
    {
        return _deadSize;
    }

    public float GetMaxBallSize()
    {
        return _maxBallSize;
    }

    public float GetStartBallSize()
    {
        return _startBallSize;
    }
}
