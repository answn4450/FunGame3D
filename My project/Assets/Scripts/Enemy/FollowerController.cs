using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoBehaviour
{
    public Transform folder;
    GameObject player;
    Vector3 originalPosition;

    void Start()
    {
        player = GameObject.Find("Player");
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            InstantiateSelf();

        FollowTarget(player.transform.position);
    }

    private void FollowTarget(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float step = Time.deltaTime * 0.01f;
        transform.position += direction.normalized * direction.magnitude * step;
    }

    private void InstantiateSelf()
    {
        Instantiate(gameObject, originalPosition, Quaternion.identity, folder);
    }
}
