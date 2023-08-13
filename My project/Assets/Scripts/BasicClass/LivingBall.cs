using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBall : MonoBehaviour
{
    public void OnGround()
    {
        Tools.GetInstance().OnGround(transform);
    }

    protected void SafeMove(Vector3 move)
    {
        float radius = transform.localScale.x * 0.5f;
        Transform underGround = Tools.GetInstance().GetUnderGround(transform).transform;
        float validX0 = Ground.GetInstance().groundX0 + radius;
        float validX1 = Ground.GetInstance().groundX1 - radius;
        float validZ0 = Ground.GetInstance().groundZ0 + radius;
        float validZ1 = Ground.GetInstance().groundZ1 - radius;
        float validY0 = Tools.GetInstance().GetTopY(underGround) + radius - (radius > 0.1f ? 0.1f : 0.0f);
        float validY1 = Ground.GetInstance().groundY1 - radius;

        float fallY = move.y;
        move = new Vector3(move.x, 0.0f, move.z);

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, move, out hit, move.magnitude + radius))
            transform.position += move;
        
        if (fallY < 0.0f)
        {
            if (Tools.GetInstance().OverTheGround(transform))
                transform.position += Vector3.up * fallY;
        }
        else
            transform.position += Vector3.up * fallY;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );
    }

}
