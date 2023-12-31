using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground
{
    private static Ground instance;

    public const float groundXZ = 1.0f;
    public float groundX0;
    public float groundX1;
    public float groundZ0;
    public float groundZ1;
    public float groundY0;
    public float groundY1;
    public float groundCenterX;
    public float groundCenterZ;

    public float groundMinimumHeight;

    public int groundWidth;
    public int groundDepth;
    public int groundHeight;

    public Vector3 groundPosition0;

    public static Ground GetInstance()
	{
        if (instance == null)
            instance = new Ground();

        return instance;
	}

    public void SetGround(GroundManager ground)
	{
        ground.transform.position += Vector3.down * ground.transform.localScale.y;

        groundWidth = (int)ground.transform.localScale.x;
        groundHeight = 7;
        groundMinimumHeight = 1.01f;
        groundDepth = (int)ground.transform.localScale.z;
        
        // ground 맨 왼쪽 밑 아래 구석.
        groundX0 = ground.transform.position.x - ground.transform.localScale.x * 0.5f;
        groundY0 = ground.transform.position.y + 0.5f;
        groundZ0 = ground.transform.position.z - ground.transform.localScale.z * 0.5f;

        groundX1 = groundX0 + groundWidth;
        groundY1 = groundY0 + groundHeight;
        groundZ1 = groundZ0 + groundDepth;

        groundCenterX = groundX0 + groundWidth * 0.5f;
        groundCenterZ = groundX0 + groundDepth * 0.5f;

        groundPosition0 = new Vector3(groundX0, groundY0, groundZ0);
    }
}
