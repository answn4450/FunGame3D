using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerBall
{
    float GetDeadSize();
    float GetStartBallSize();
    float GetMaxBallSize();
    string GetBallName();
}