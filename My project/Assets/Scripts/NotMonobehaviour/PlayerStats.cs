using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    //public static int a = 3;
    private static PlayerStats Instance = null;

    public static PlayerStats GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PlayerStats();
        }
        return Instance;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
