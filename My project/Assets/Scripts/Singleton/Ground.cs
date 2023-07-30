using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground
{
    private static Ground instance;

    public float groundX0;
    public float groundX1;
    public float groundZ0;
    public float groundZ1;
    public float groundY0;
    public float groundY1;

    public int groundWidth;
    public int groundDepth;
    public int groundHeight;

    public static Ground GetInstance()
	{
        if (instance == null)
            instance = new Ground();

        return instance;
	}

    public void SetGroundWithPannel(GroundManager ground)
	{
        groundWidth = (int)ground.transform.localScale.x;
        groundHeight = 7;
        groundDepth = (int)ground.transform.localScale.z;
        
        // ground �� ���� �� �Ʒ� ����.
        groundX0 = ground.transform.position.x - ground.transform.localScale.x * 0.5f;
        groundY0 = ground.transform.position.y - 0.5f;
        groundZ0 = ground.transform.position.z - ground.transform.localScale.z * 0.5f;

        groundX1 = groundX0 + groundWidth;
        groundY1 = groundY0 + groundHeight;
        groundZ1 = groundZ0 + groundDepth;
    }

    public int GetIndexX(float positionX)
    {
        int indexX =  (int)(positionX - groundX0);
        return Mathf.Clamp(indexX, 0, groundWidth - 1);
    }

    public int GetIndexZ(float positionZ)
    {
        int indexZ = (int)(positionZ - groundZ0);
        return Mathf.Clamp(indexZ, 0, groundDepth - 1);
    }

    public int GetIndexY(float positionY)
    {
        int indexY = (int)(positionY - groundY0);
        return Mathf.Clamp(indexY, 0, groundHeight - 1);
    }

    public Vector3 GetIndexPosition(Vector3 position)
	{
        return new Vector3(
            groundX0 + GetIndexX(position.x) + 0.5f,
            groundY0 + GetIndexY(position.y) + 0.5f,
            groundZ0 + GetIndexZ(position.z) + 0.5f
            );
	}
}
