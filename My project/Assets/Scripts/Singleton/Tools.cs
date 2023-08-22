using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public LayerMask groundMask = LayerMask.GetMask("Ground");
    // 소수점 2자리 수 까지 사용
    private readonly int minor = 2;
    public float groundLiftMinorDiffrence = 2.384186E-07f;
    private static Tools instance;

    public static Tools GetInstance()
	{
        if (instance == null)
            instance = new Tools();

        return instance;
	}

    public int GetGroundIndexX(float positionX)
    {
        int indexX = (int)(positionX - Ground.GetInstance().groundX0);
        return Mathf.Clamp(indexX, 0, Ground.GetInstance().groundWidth - 1);
    }

    public int GetGroundIndexZ(float positionZ)
    {
        int indexZ = (int)(positionZ - Ground.GetInstance().groundZ0);
        return Mathf.Clamp(indexZ, 0, Ground.GetInstance().groundDepth - 1);
    }

    public int GetGroundIndexY(float positionY)
    {
        int indexY = (int)(positionY - Ground.GetInstance().groundY0);
        return Mathf.Clamp(indexY, 0, Ground.GetInstance().groundHeight - 1);
    }

    public bool SameGround(Transform a, Transform b)
	{
        int aX = GetGroundIndexX(a.position.x);
        int aZ = GetGroundIndexZ(a.position.z);
        int bX = GetGroundIndexX(b.position.x);
        int bZ = GetGroundIndexZ(b.position.z);

        return (aX == bX && aZ == bZ);
	}

    public bool BallOutGround(Transform ball)
    {
        float radius = ball.localScale.x * 0.5f;

        if (ball.position.x - radius < Ground.GetInstance().groundX0)
            return true;
        if (ball.position.x + radius > Ground.GetInstance().groundX1)
            return true;
        if (ball.position.y - radius < Ground.GetInstance().groundY0)
            return true;
        if (ball.position.y + radius < Ground.GetInstance().groundY1)
            return true;
        if (ball.position.z - radius < Ground.GetInstance().groundZ0)
            return true;
        if (ball.position.z + radius < Ground.GetInstance().groundZ1)
            return true;
        else
            return false;
    }

    public bool OverTheGround(Transform transform)
    {
        GroundController ground = GetUnderGround(transform);
        if (ground)
        {
            float diff = GetBottomY(transform) - GetTopY(ground.transform);
            if (diff > 0.0f)
            {
                return !MinorEqual(diff, 0.0f);
            }
            else
                return false;
        }
        else
            return true;

    }

    public bool MinorEqual(float a, float b)
    {
        return MinorFloat(a).Equals(MinorFloat(b));
    }

    public bool GetBallTouchRect(Transform ball, Transform rect)
    {
        return true;
    }

    public void AddGroundCollider(Transform transform)
    {
        GroundController ground = GetUnderGround(transform);
        if (ground)
            ground.AddCollider(transform);
    }

    public void AddGroundUpper(Transform transform)
    {
        if (!Status.GetInstance().groundUppers.Contains(transform))
            Status.GetInstance().groundUppers.Add(transform);
    }

    public float GetHeight(Transform transform)
    {
        if (transform.GetComponent<LivingBall>())
            return transform.localScale.x;
        else
            return transform.localScale.y;
    }

    public float GetBottomY(Transform transform)
    {
        float halfHeight = GetHeight(transform) * 0.5f;
        return transform.position.y - halfHeight;
    }

    public float GetTopY(Transform transform)
    {
        float halfHeight = GetHeight(transform) * 0.5f;
        return transform.position.y + halfHeight;
    }

    public float PaddingGroundDistance(Transform transform)
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");
        bool cast = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, transform.transform.localScale.x * 0.5f, groundMask);
        GroundController ground = GetUnderGround(transform);
        float groundTop = Tools.GetInstance().GetTopY(ground.transform);
        float playerBottom = Tools.GetInstance().GetBottomY(transform);
        float distanceY = playerBottom - groundTop;
        //Debug.LogFormat("cast: {0}, distance : {1}, {2}", cast, distanceY, transform.position.y - groundTop);

        return distanceY;
    }

    public float PlayerCastBottomGround()
    {
        Transform player = GameObject.Find("Player").transform;
        float distance = PaddingGroundDistance(player);
        return distance;
    }

    public float GetDistanceXZ(Transform a, Transform b)
    {
        Vector3 diff = a.position - b.position;
        diff.y = 0.0f;

        return diff.magnitude;
    }

    public float MinorFloat(float a)
    {
        return (float)(decimal.Round((decimal)a, minor));
    }

    public GroundController GetUnderGround(Transform transform)
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        Vector3 topY = new Vector3(
            transform.position.x,
            Ground.GetInstance().groundY1 + 1.0f,
            transform.position.z
            );

        if (Physics.Raycast(topY, Vector3.down, out hit, Mathf.Infinity, groundMask))
            return hit.transform.GetComponent<GroundController>();
        else
            return null;
    }

    public Vector3 GetDirectionXZ(Transform a, Transform b)
    {
        Vector3 diff = a.position - b.position;
        diff.y = 0.0f;

        return diff;
    }

    public Vector3 GetGroundIndexPosition(Vector3 position)
    {
        return new Vector3(
            Ground.GetInstance().groundX0 + GetGroundIndexX(position.x) + 0.5f,
            Ground.GetInstance().groundY0 + GetGroundIndexY(position.y) + 0.5f,
            Ground.GetInstance().groundZ0 + GetGroundIndexZ(position.z) + 0.5f
            );
    }
    
    public Vector3 GetSphereVector3(float r)
    {
        return new Vector3(r, r, r);
    }

    public void FloatBugTest()
    {
        float a = 0.0123456789f;
        float c = 0.0000000001f;
        float b = a + c;
        float d = a + c;

        //Debug.Log($"{(a + c) == b}, {a}, {b}, {(b-a) == c }");
    }

    public void Test()
    {
        Transform player = GameObject.Find("Player").transform;
        float bottom = Tools.GetInstance().GetBottomY(player);
        GroundController ground = GetUnderGround(player);
        float top = Tools.GetInstance().GetTopY(ground.transform);
        //Debug.Log(bottom - top);
    }

    public void ImplementBug(string targetName)
    {
        GameObject target = GameObject.Find(targetName);
        Tools.GetInstance().AddGroundCollider(target.transform);
    }

    public void TraceGroundBug(string name)
    {
        Transform target = GameObject.Find(name).transform;
        //bool stick = StickGround(target);
        bool inAir = target.GetComponent<LivingBall>().InAir();
        if (inAir)
            Debug.Log(Mathf.Approximately(GetGroundInterval(target), 0.0f));
    }

    public bool TraceBugKey()
    {
        return Input.GetKeyDown(KeyCode.F) || true;
    }

    public float GetGroundInterval(Transform target)
    {
        GroundController ground = GetInstance().GetUnderGround(target);
        float groundTopY = GetInstance().GetTopY(ground.transform);
        float targetBottomY = GetInstance().GetBottomY(target);
        return targetBottomY - groundTopY;
    }

    public float GetUnderGroundEmptyHeight(Transform target)
    {
        GroundController ground = Tools.GetInstance().GetUnderGround(target);
        return ground.GetEmptyHeight();
    }

    public float GetYAngle(Transform start, Transform end)
    {
        Vector3 diff = end.position - start.position;
        return Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
    }
    
    private bool StickGround(Transform target)
    {
        Transform downGround = Tools.GetInstance().GetUnderGround(target).transform;
        float groundTopY = Tools.GetInstance().GetTopY(downGround);
        float radius = target.localScale.x * 0.5f;
        float finalY = groundTopY + radius;
        finalY = (float)decimal.Round((decimal)finalY, 2);
        target.position = new Vector3(
            target.position.x,
            finalY,
            target.position.z
            );

        float diff = target.position.y - finalY;
        if (!MinorEqual(finalY, target.position.y))
            Debug.Log($"StickGround: {diff}, {finalY}, {target.position.y}");
        return MinorEqual(finalY, target.position.y);
    }

        /*
    public Vector3 CircleToRectCollision(Vector3 movement, Transform circle, Transform rect)
    {
         GetTopY(rect);
        bool detect = false;
        RaycastHit[] hits = Physics.RaycastAll(circle.transform.position, movement, movement.magnitude);
        foreach(RaycastHit hit in hits)
        {
            if (hit.transform == rect)
                detect = true;
        }

        if (detect)
            return movement;
        else
        {

        }
    }
        */
}
