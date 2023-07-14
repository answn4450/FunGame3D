using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour
{
    /*
    public ItemController itemController;
    private PlayerController player;
    private float power;
    private Vector3 direction;
    
    void Awake()
    {
        power = 9.8f;
        direction = transform.localRotation * Vector3.up;
    }

    void Start()
    {
        player = itemController.player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            //player.AffectPower(Vector3.up * 24.0f);
            StartCoroutine(Push());
        }
    }

    IEnumerator Push()
    {
        while (power < 9.8f + 6.0f)
        {
            yield return null;
            player.AffectPower(direction * power * power);
            power += Time.deltaTime * 5;
        }
        power = 9.8f;
    }
    */
}
