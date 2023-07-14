using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private PlayerController player;

    private Vector3 direction;
    private int state;
    private float power;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        power = 3.0f;
        direction = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Player")
        {
            if (state == 1)
                direction = Vector3.up;
            else if (state == 2)
                direction = transform.localRotation.y * Vector3.forward;
            
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
}
