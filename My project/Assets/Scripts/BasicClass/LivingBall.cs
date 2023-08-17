using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBall : MonoBehaviour
{

    public void OnGround()
    {
        Tools.GetInstance().AddGroundCollider(transform);
        Tools.GetInstance().AddGroundUpper(transform);
    }

    protected void SafeMove(Vector3 move)
    {
        float radius = transform.localScale.x * 0.5f;
        float validX0 = Ground.GetInstance().groundX0 + radius;
        float validX1 = Ground.GetInstance().groundX1 - radius;
        float validZ0 = Ground.GetInstance().groundZ0 + radius;
        float validZ1 = Ground.GetInstance().groundZ1 - radius;
        float validY0 = Ground.GetInstance().groundY0 + radius;
        float validY1 = Ground.GetInstance().groundY1 - radius;

        Vector3 oriPosition = transform.position;
        RaycastHit firstHit;
        RaycastHit hit;

        if (Physics.Raycast(oriPosition, move, out firstHit, move.magnitude + radius))
        {
            if (Physics.Raycast(oriPosition, move, out hit, move.magnitude + radius, LayerMask.GetMask("Ground")))
            {
                if (firstHit.transform == hit.transform)
                {
                    float groundTopY = Tools.GetInstance().GetTopY(hit.transform);
                    if (move.y < 0.0f && transform.position.y + move.y - radius <= groundTopY)
                    {
                        transform.position = new Vector3(
                        transform.position.x + move.x,
                        groundTopY + radius,
                        transform.position.z + move.z
                        );
                    }
                }
            }
        }
        else
            transform.position += move;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );

    }

    public bool InAir()
    {
        bool castBlock = (Physics.Raycast(transform.position, Vector3.down, transform.localScale.x * 0.5f+ 1.0f));
        bool overTheGround = Tools.GetInstance().OverTheGround(transform);
        return !castBlock && overTheGround;
    }
}
