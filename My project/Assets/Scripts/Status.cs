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
}
