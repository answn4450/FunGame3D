using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManualKey;


public class Status
{
    private static Status Instance = null;
    public static Status GetInstance()
	{
        if (Instance == null)
            Instance = new Status();
        return Instance;
	}

    public bool gotPlaySet = false;

    public bool endGame = false;
    
    public float groundX0;
    public float groundX1;
    public float groundZ0;
    public float groundZ1;
    public float groundY;

    public int structureUse = 0;
    public int structureMaxUse = 0;
    public int maxStage = 3;
    public int currentStage = 0;
    public bool elevatorInScene = false;
    public ElevatorController prevElevator = null;
    public ElevatorController nextElevator = null;

    public KeyboardARow spaceKey = KeyboardARow.Shot;
}
