using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Transform core;
    public Transform hover;

    private void Update()
    {
        hover.LookAt(core);
    }
    
}
