using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySetController : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
