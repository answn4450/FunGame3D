using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        transform.localPosition = Vector3.down;
        transform.position = Vector3.up;
    }

}