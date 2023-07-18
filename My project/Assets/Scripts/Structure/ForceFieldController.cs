using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldController : MonoBehaviour
{
    GameObject destroyFX;

    private void Start()
    {
        destroyFX = PrefabManager.GetInstance().GetPrefabByName("CFXR3 Hit Ice B (Air)");
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject effect = Instantiate(destroyFX);
        effect.transform.position = other.transform.position;

        GameObject.Destroy(other.gameObject);
    }
}
