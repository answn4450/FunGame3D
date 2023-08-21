using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : Structure
{
    private PlayerController player;
    private float power;
    private Vector3 direction;
    
    void Awake()
    {
        power = 0.0f;
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
        while (power < 9.8f + 3.0f)
        {
            yield return null;
            player.AffectPower(direction * power);
            power += Time.deltaTime;
        }
        power = 9.8f;
    }
}
