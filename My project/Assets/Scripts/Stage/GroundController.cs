using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    private GameObject rightWall;
    private GameObject forwardWall;
    private GameObject leftWall;
    private GameObject backWall;


    private void Awake()
    {
        float width = Status.GetInstance().groundWidth;
        float height = Status.GetInstance().groundHeight;

        rightWall = GenerateWall("rightWall");
        forwardWall = GenerateWall("forwardWall");
        leftWall = GenerateWall("leftWall");
        backWall = GenerateWall("backWall");
        
        SetSize(width, height);
    }

    private GameObject GenerateWall(string name)
    {
        GameObject wall = Instantiate(
            PrefabManager.GetInstance().GetPrefabByName("Wall1x1")
            );

        wall.name = name;

        return wall;
    }

    public void SetSize(float width, float height)
    {
        transform.localScale = new Vector3(
            width,
            1.0f,
            height
            );

        transform.position = Vector3.zero + Vector3.down * 0.5f;
        
        rightWall.transform.position = (Vector3.right * width + Vector3.down) * 0.5f;
        forwardWall.transform.position = (Vector3.forward * height + Vector3.down) * 0.5f;
        leftWall.transform.position = (Vector3.left * width + Vector3.down) * 0.5f;
        backWall.transform.position = (Vector3.back * height + Vector3.down) * 0.5f;

        rightWall.transform.localScale = new Vector3(1.0f, 1.0f, height);
        forwardWall.transform.localScale = new Vector3(width, 1.0f, 1.0f);
        leftWall.transform.localScale = new Vector3(1.0f, 1.0f, height);
        backWall.transform.localScale = new Vector3(width, 1.0f, 1.0f);
    }
}
