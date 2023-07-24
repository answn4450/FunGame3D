using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour
{
    private PlayerController player;
    private float power;
    private Vector3 direction;
    
    void Awake()
    {
        power = 9.8f;
        direction = transform.localRotation * Vector3.up;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            player = other.GetComponent<PlayerController>();
            StartCoroutine(Push());
        }
    }

    IEnumerator Push()
    {
        while (power < 9.8f + 1.0f)
        {
            yield return null;
            player.AffectPower(direction * power * power * 0.6f);
            power += Time.deltaTime * 2;
        }
        power = 9.8f;
    }
}
