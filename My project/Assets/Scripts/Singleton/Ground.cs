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
        
        // ground ¸Ç ¿ÞÂÊ ¹Ø ¾Æ·¡ ±¸¼®.
        groundX0 = ground.transform.position.x - ground.transform.localScale.x * 0.5f;
        groundY0 = ground.transform.position.y - 0.5f;
        groundZ0 = ground.transform.position.z - ground.transform.localScale.z * 0.5f;

        groundX1 = groundX0 + groundWidth;
        groundY1 = groundY0 + groundHeight;
        groundZ1 = groundZ0 + groundDepth;
    }
}
