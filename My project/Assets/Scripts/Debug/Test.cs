using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StructureCollision;

public class Test : MonoBehaviour
{
    public Test child;
    public int b;

    private void Start()
    {
        StartCoroutine(a());
    }

    IEnumerator a()
    {
        while (true)
        {
            yield return null;
        }
    }
}
