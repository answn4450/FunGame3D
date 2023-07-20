using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private bool peak;
    void Start()
    {
        peak = false;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            other.GetComponent<PlayerController>().OnElevator(gameObject);
            On();
        }
    }

    IEnumerator On()
    {
        while (!(peak && transform.position.y > 0.0f))
        {
            yield return null;
            transform.position += Vector3.up * Time.deltaTime;
            if (transform.position.y > 10.0f)
            {
                peak = true;
                transform.position -= Vector3.down * 10.0f;
            }
        }
    }
}
