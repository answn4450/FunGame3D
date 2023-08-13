using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbag : MonoBehaviour
{
    public bool satisfy;
    private GameObject signFx;

    private void Awake()
    {
        satisfy = false;
        signFx = PrefabManager.GetInstance().GetPrefabByName("CFXR Fire");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletController>())
        {
            if (!satisfy)
                Instantiate(signFx, transform).transform.position = transform.position;
            satisfy = true;
        }
    }
}
