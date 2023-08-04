using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public LayerMask groundMask = LayerMask.GetMask("Ground");
    private static Tools instance;

    public static Tools GetInstance()
	{
        if (instance == null)
            instance = new Tools();

        return instance;
	}

    public int GetGroundIndexX(float positionX)
    {
        int indexX = (int)(positionX - Ground.GetInstance().groundX0);
        return Mathf.Clamp(indexX, 0, Ground.GetInstance().groundWidth - 1);
    }

    public int GetGroundIndexZ(float positionZ)
    {
        int indexZ = (int)(positionZ - Ground.GetInstance().groundZ0);
        return Mathf.Clamp(indexZ, 0, Ground.GetInstance().groundDepth - 1);
    }

    public int GetGroundIndexY(float positionY)
    {
        int indexY = (int)(positionY - Ground.GetInstance().groundY0);
        return Mathf.Clamp(indexY, 0, Ground.GetInstance().groundHeight - 1);
    }

    public Vector3 GetGroundIndexPosition(Vector3 position)
    {
        return new Vector3(
            Ground.GetInstance().groundX0 + GetGroundIndexX(position.x) + 0.5f,
            Ground.GetInstance().groundY0 + GetGroundIndexY(position.y) + 0.5f,
            Ground.GetInstance().groundZ0 + GetGroundIndexZ(position.z) + 0.5f
            );
    }

    public bool SameGround(Transform a, Transform b)
	{
        int aX = GetGroundIndexX(a.position.x);
        int aZ = GetGroundIndexZ(a.position.z);
        int bX = GetGroundIndexX(b.position.x);
        int bZ = GetGroundIndexZ(b.position.z);

        return (aX == bX && aZ == bZ);
	}

    public bool BallOutGround(Transform ball)
    {
        float radius = ball.localScale.x * 0.5f;

        if (ball.position.x - radius < Ground.GetInstance().groundX0)
            return false;
        if (ball.position.x + radius > Ground.GetInstance().groundX1)
            return false;
        if (ball.position.y - radius < Ground.GetInstance().groundY0)
            return false;
        if (ball.position.y + radius < Ground.GetInstance().groundY1)
            return false;
        if (ball.position.z - radius < Ground.GetInstance().groundZ0)
            return false;
        if (ball.position.z + radius < Ground.GetInstance().groundZ1)
            return false;
        else
            return true;
    }

    public bool GetBallTouchRect(Transform ball, Transform rect)
    {
        return true;
    }
}
