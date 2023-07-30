using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    private List<List<GroundController>> groundList;
    private GameObject groundPrefab;
    private Transform groundFolder;

    private int sizeX, sizeY, sizeZ;
    float x0, y0, z0;


	private void Awake()
	{
        groundFolder = new GameObject("GroundFolder").transform;
	}

	public void CreateGrounds()
    {
        groundPrefab = PrefabManager.GetInstance().GetPrefabByName("Ground");

        sizeX = (int)transform.localScale.x;
        sizeZ = (int)transform.localScale.z;
        sizeY = 8;
        x0 = Ground.GetInstance().groundX0 + 0.5f;
        y0 = Ground.GetInstance().groundY0 + 0.5f;
        z0 = Ground.GetInstance().groundZ0 + 0.5f;

        groundList = new List<List<GroundController>>();


        for (int x = 0; x < sizeX; ++x)
        {
            List<GroundController> groundZLine = new List<GroundController>();
            for (int z = 0; z < sizeZ; ++z)
            {
                GroundController ground;
                ground = Instantiate(groundPrefab).GetComponent<GroundController>();
                ground.transform.SetParent(groundFolder);
                ground.name = "grond " + x.ToString() + " " + z.ToString();
                ground.SetWithIndex(x, z);
                groundZLine.Add(ground);
            }
            groundList.Add(groundZLine);
        }
    }

    public void UpDownOrSqueeze(PlayerController player)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int z = 0; z < sizeZ; ++z)
            {
                //groundList[x][z].UpDownByTarget(player.transform);
                groundList[x][z].UpDownOrSqueeze(player);
            }
        }
    }

    private GroundController FindGround(Vector3 position)
    {
        int x = (int)(position.x - x0 - 0.5f);
        int z = (int)(position.z - z0 - 0.5f);
        if ((x >= 0 && x < sizeX) || (z >= 0 && z < sizeY))
            return groundList[x][z];
        else
            return null;
    }
}
