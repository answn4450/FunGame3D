using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    private static Tools instance;

    public static Tools GetInstance()
	{
        if (instance == null)
            instance = new Tools();

        return instance;
	}

    public bool SameGround(Transform a, Transform b)
	{
        int aX = GetGroundIndexX(a.position.x);
        int aZ = GetGroundIndexX(a.position.z);
        int bX = GetGroundIndexX(b.position.x);
        int bZ = GetGroundIndexX(b.position.z);

        return (aX == bX && aZ == bZ);
	}

    public int GetGroundIndexX(float positionX)
    {
        return (int)(positionX - Status.GetInstance().groundX0);
    }

    public int GetGroundIndexZ(float positionZ)
    {
        return (int)(positionZ - Status.GetInstance().groundZ0);
    }
}
