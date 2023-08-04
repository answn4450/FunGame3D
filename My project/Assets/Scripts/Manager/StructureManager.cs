using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public void LoopStructuresInFolder()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject structure = transform.GetChild(i).gameObject;
            string name = structure.name;
            if (name == "StopAura")
                structure.GetComponent<StopAuraController>().StopPlayerBlock();
        }
        Fall();
    }

    public void Fall()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject structure = transform.GetChild(i).gameObject;
            structure.GetComponent<Structure>().Fall();
        }
    }

    public List<GroundController> GetLowerStructures()
    {
        List<GroundController> lowerStructures = new List<GroundController>();
        float lowerY = Ground.GetInstance().groundY0;
        float maximumHeight = Ground.GetInstance().groundHeight;
        for (int i = 0; i < transform.childCount; ++i)
        {
            RaycastHit hit;
            GameObject structure = transform.GetChild(i).gameObject;
            Vector3 bottomPos = new Vector3(
                structure.transform.position.x,
                lowerY,
                structure.transform.position.z
                );
            if (Physics.Raycast(bottomPos, Vector3.up, out hit, maximumHeight))
            {
                if (hit.transform.GetComponent<GroundController>() && !lowerStructures.Contains(hit.transform.GetComponent<GroundController>()))
                    lowerStructures.Add(hit.transform.GetComponent<GroundController>());
            }
        }

        return lowerStructures;
    }
    
}
