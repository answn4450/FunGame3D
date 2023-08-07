using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBall : MonoBehaviour
{
    public void OnGround()
    {
        Tools.GetInstance().OnGround(gameObject);
    }

    protected void SafeMove(Vector3 move)
    {
        float radius = transform.localScale.x * 0.5f;
        float validX0 = Ground.GetInstance().groundX0 + radius;
        float validX1 = Ground.GetInstance().groundX1 - radius;
        float validZ0 = Ground.GetInstance().groundZ0 + radius;
        float validZ1 = Ground.GetInstance().groundZ1 - radius;
        float validY0 = Ground.GetInstance().groundY0 + Ground.GetInstance().groundMinimumHeight + radius;
        float validY1 = Ground.GetInstance().groundY1 - radius;

        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        bool dontMove = false;
        if (Physics.Raycast(transform.position, move, out hit, move.magnitude + radius, groundMask))
        {
            bool moveDown = move.y < 0;
            float groundTopY = hit.transform.position.y + hit.transform.localScale.y * 0.5f;
            if (moveDown)
                move += Vector3.down * (groundTopY - transform.position.y + radius);
            else
                dontMove = true;
        }
        if (!dontMove)
            transform.position += move;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );
    }

}
