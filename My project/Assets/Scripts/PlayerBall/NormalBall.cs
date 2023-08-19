using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBall : PlayerBall, IPlayerBall
{
    public string ballName {
        get
        {
            return "Normal Ball";
        }
        set 
        {
            Debug.LogWarning($"Ball 이름 {value} 로 변경 불허용." );
        }
    }

    public float deadSize = 2.0f;

    public string GetBallName()
    {
        return ballName;
    }

    public float GetDeadSize()
    {
        return deadSize;
    }
}
