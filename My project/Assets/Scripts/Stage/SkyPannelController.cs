using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyPannelController : MonoBehaviour
{
    private const string blockShader = "Legacy Shaders/Transparent/Specular";

    private void Awake()
    {
        GetComponent<Renderer>().material = new Material(Shader.Find(blockShader));
    }

    public void SetSkyPannelByGroundPannel(GroundManager groundManager)
    {
        float fullHeight = Ground.GetInstance().groundHeight;
        transform.position = groundManager.transform.position + Vector3.up * (fullHeight + 1.0f); 
        transform.localScale = groundManager.transform.localScale;

        SetTransparency(0.1f);
    }

    public void TransparentByPlayerY(PlayerController player)
    {
        float leftHeight = Ground.GetInstance().groundY1 - player.transform.position.y;
        if (leftHeight < 3.0f)
            SetTransparency(1 - leftHeight * 0.2f);
    
    }

    private void SetTransparency(float a)
    {
        Color color = GetComponent<Renderer>().material.color;
        color.a = a;
        GetComponent<Renderer>().material.color = color;
    }
}
