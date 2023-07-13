using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform to;
    
    void Update()
    {
        float deg = Vector3.SignedAngle(transform.position, to.position, Vector3.forward);
        deg = Quaternion.FromToRotation(Vector3.up, to.position - transform.position).eulerAngles.z;
        //deg = Vector3.SignedAngle(transform.up, to.position - transform.position, Vector3.left);
        Debug.Log(deg);
    }

}