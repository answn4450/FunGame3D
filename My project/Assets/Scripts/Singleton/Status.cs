using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Status
{
    private static Status Instance = null;
    private string savePath = "Assets/Resources/Saves.txt";

    public static Status GetInstance()
	{
        if (Instance == null)
            Instance = new Status();
        return Instance;
	}

    public bool endGame = false;

    public int maxStage = 3;
    public int currentStage = 1;
    public bool elevatorInScene = false;
    public float groundUpDownWeakPower = 1.0f;
    public float groundUpDownStrongPower = 2.0f;
    public ElevatorController prevElevator = null;
    public ElevatorController nextElevator = null;
    public List<Transform> groundUppers = new List<Transform>();

    public void Read()
    {
        string recordStage = File.ReadAllText(savePath);
        currentStage = int.Parse(recordStage);
    }

    public void Save()
    {
        File.WriteAllText(savePath, currentStage.ToString());
    }
}
