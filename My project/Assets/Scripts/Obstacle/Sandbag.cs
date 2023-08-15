using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbag : MonoBehaviour
{
    public bool satisfy;
    private GameObject signFX;

    private void Awake()
    {
        satisfy = false;
        signFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Fire");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletController>())
        {
            if (!satisfy)
                Instantiate(signFX, transform).transform.position = transform.position;
            satisfy = true;
        }
    }
}
