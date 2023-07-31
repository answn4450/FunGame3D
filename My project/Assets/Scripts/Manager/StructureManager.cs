using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structure;

public class StructureManager : MonoBehaviour
{
    public void LoopStructuresInFolder(Transform folder)
    {
        int size = folder.transform.childCount;
        
        for (int i = 0; i < size; ++i)
        {
            GameObject child = folder.transform.GetChild(i).gameObject;
            string name = child.name;
            if (name == "StopAura")
                child.GetComponent<StopAuraController>().StopPlayerBlock();
        }
    }
}
