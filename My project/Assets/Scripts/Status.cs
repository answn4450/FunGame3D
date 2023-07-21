using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    private static Status Instance = null;
    public static Status GetInstance()
	{
        if (Instance == null)
            Instance = new Status();
        return Instance;
	}

    public bool endGame = false;
    public float groundWidth = 5.0f;
    public float groundHeight = 5.0f;
    public int structureUse = 0;
    public int structureMaxUse = 0;
    public int maxStage = 3;
    public int currentStage = 0;
    public bool elevatorInScene = false;
    public ElevatorController prevElevator = null;
    public ElevatorController nextElevator = null;
}
