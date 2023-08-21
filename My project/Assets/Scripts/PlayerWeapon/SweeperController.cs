using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweeperController : MonoBehaviour
{
    Vector3 forward;

    private void Awake()
    {
        Tools.GetInstance().AddGroundUpper(transform);
    }
        
    void Update()
    {
        GroundController underGround = Tools.GetInstance().GetUnderGround(transform);
        if (underGround)
        {
            BindPosition(underGround);
            Tools.GetInstance().AddGroundCollider(transform);
        }
        else
            Destroy(gameObject);
        NoCollideMove(forward * Time.deltaTime);
        NoCollideMove(Vector3.down * Time.deltaTime);
    }

    public void Set(Transform from)
    {
        transform.SetPositionAndRotation(from.position, from.rotation);
        forward = transform.forward;
    }

    public bool InAir()
    {
        bool castBlock = (Physics.Raycast(transform.position, Vector3.down, transform.localScale.x * 0.5f));
        bool overTheGround = Tools.GetInstance().OverTheGround(transform);
        return !castBlock && overTheGround;
    }

    public void NoCollideMove(Vector3 move)
    {
        float radius = transform.localScale.x * 0.5f;

        Vector3 oriPosition = transform.position;
        bool blocked = Physics.Raycast(oriPosition, move, move.magnitude + radius + 0.1f);
        if (!blocked)
            transform.position += move;
    }

    private void BindPosition(GroundController underGround)
    {
        float radius = transform.localScale.x * 0.5f;
        float validX0 = Ground.GetInstance().groundX0;
        float validX1 = Ground.GetInstance().groundX1;
        float validZ0 = Ground.GetInstance().groundZ0;
        float validZ1 = Ground.GetInstance().groundZ1;
        float validY0 = Tools.GetInstance().GetTopY(underGround.transform);
        float validY1 = Ground.GetInstance().groundY1;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, validX0, validX1),
            Mathf.Clamp(transform.position.y, validY0, validY1),
            Mathf.Clamp(transform.position.z, validZ0, validZ1)
            );
    }
}
