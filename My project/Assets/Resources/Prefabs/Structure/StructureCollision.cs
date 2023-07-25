using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StructureCollision
{
    public class BackCollision : MonoBehaviour
    {
        public void Back(Transform transform, GameObject other)
        {
            Vector3 direction = other.transform.position - transform.position;
            float dist = direction.magnitude;
            other.transform.position = transform.position + (dist + 1.2f) * direction;
            Debug.Log("a");
        }
    }
}