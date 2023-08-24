using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Vector3 core = Vector3.zero;
    public Transform coreTransform;
    public Vector3 hover = Vector3.zero;
    public Transform hoverTransform;

    public void Update()
    {
        core = coreTransform.position;
        hover = hoverTransform.position;
    }
    
}
