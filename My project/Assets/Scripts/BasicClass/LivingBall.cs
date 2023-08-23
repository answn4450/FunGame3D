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

    public bool InAir()
    {
        bool castBlock = (Physics.Raycast(transform.position, Vector3.down, transform.localScale.x * 0.5f));
        bool overTheGround = Tools.GetInstance().OverTheGround(transform);
        return !castBlock && overTheGround;
    }

    public void SafeMove(Vector3 move)
    {
        float radius = transform.localScale.x * 0.5f;

        Vector3 oriPosition = transform.position;

        if (Physics.Raycast(oriPosition, move, out RaycastHit firstHit, move.magnitude + radius))
        {
            // Debug.Log("cast under stuff");
            if (Physics.Raycast(oriPosition, move, out RaycastHit hit, move.magnitude + radius, LayerMask.GetMask("Ground")))
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

        BindPosition();
    }

    public List<GroundController> GetTouchingGrounds()
    {
        float radius = transform.localScale.x * 0.5f;
        //float minor = Tools.GetInstance().groundLiftMinorDiffrence;
        float minor = 0.01f;
        List<GroundController> touch = new List<GroundController>();
        List<Vector3> search = new List<Vector3> { 
            Vector3.up, Vector3.down, 
            Vector3.right, Vector3.left, 
            Vector3.forward, Vector3.back
        };

        foreach (Vector3 direction in search)
        {
            RaycastHit[] newHit;
            newHit = Physics.RaycastAll(transform.position, direction, radius + minor, LayerMask.GetMask("Ground"));
            foreach(RaycastHit hit in newHit)
            {
                if (!touch.Contains(hit.transform.GetComponent<GroundController>()))
                {
                    touch.Add(hit.transform.GetComponent<GroundController>());
                }
            }
        }

        return touch;
    }

    private void BindPosition()
    {
        float radius = transform.localScale.x * 0.5f;
        float validX0 = Ground.GetInstance().groundX0 + radius;
        float validX1 = Ground.GetInstance().groundX1 - radius;
        float validZ0 = Ground.GetInstance().groundZ0 + radius;
        float validZ1 = Ground.GetInstance().groundZ1 - radius;
        float validY0 = Ground.GetInstance().groundY0 + radius;
        float validY1 = Ground.GetInstance().groundY1 - radius;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );
    }
}
