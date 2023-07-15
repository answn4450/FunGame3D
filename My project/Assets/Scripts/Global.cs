using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    private static Global Instance = null;
    public static Global GetInstance()
	{
        if (Instance == null)
            Instance = new Global();
        return Instance;
	}
}
