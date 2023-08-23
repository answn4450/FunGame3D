using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Vector3 core;
    public Transform hover;

    public void Update()
    {
        hover.LookAt(core);
    }
    
}
