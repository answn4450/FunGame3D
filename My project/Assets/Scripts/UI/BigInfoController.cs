using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigInfoController : MonoBehaviour
{
    private float size;
    private float destSize;
    private void Awake()
    {
        size = GetComponent<Text>().fontSize * 0.1f;
        destSize = size;
    }

    private void LateUpdate()
    {
        float t = Mathf.Clamp(Time.deltaTime, 0.01f, 0.1f);
        size = Mathf.Lerp(size, destSize, t);
        GetComponent<Text>().fontSize = (int)size;
    }

    public void GetNewText(string text)
    {
        GetComponent<Text>().text = text;
        destSize = 90.0f;
        size = 110.0f;
    }
}
