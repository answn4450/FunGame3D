using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public void LoopStructuresInFolder()
    {
        int size = transform.childCount;
        
        for (int i = 0; i < size; ++i)
        {
            GameObject child = transform.GetChild(i).gameObject;
            string name = child.name;
            if (name == "StopAura")
                child.GetComponent<StopAuraController>().StopPlayerBlock();
        }
        Fall();
    }

    public void Fall()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponent<Structure>().Fall();
        }
    }
}
