using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    public int collidersNumber;
    public bool temporailyStop;
    
    private Color originalColor;
    public List<float> collidersHeightRangeBottom = new List<float>();
    public List<float> collidersHeightRangeTop = new List<float>();
    private List<Transform> colliders;
    private const float defaultSpeed = 0.2f;
    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    public float heightDestination;
    //private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        collidersNumber = 0;
        colliders = new List<Transform>();
        temporailyStop = false;
        //slowdownPlayer = false;
        originalColor = GetComponent<Renderer>().material.color;
        heightDestination = 0.0f;
    }

    public void Reset()
    {
        temporarilySpeed = defaultSpeed;
        SetColor(originalColor);
    }

    public void EraseStats()
    {
        collidersNumber = 0;
    }

    public void AddCollider(Transform collider)
    {
        if (colliders.Count == collidersNumber)
            colliders.Add(collider);
        else
            colliders[collidersNumber] = collider;

        collidersNumber++;
    }

    public void SetWithIndex(int indexX, int indexZ)
    {
        groundX = Ground.GetInstance().groundX0 + indexX + 0.5f;
        groundZ = Ground.GetInstance().groundZ0 + indexZ + 0.5f;

        transform.position = new Vector3(
            groundX,
            Ground.GetInstance().groundY0 + 0.5f,
            groundZ
            );
    }

    public void SlowdownPlayer()
    {
        //slowdownPlayer = true;
    }

    public void ResetTemporarilyOptions()
    {
        temporarilySpeed = defaultSpeed;
        temporailyStop = false;
        //slowdownPlayer = false;
    }

    public void SetGroundTemporarilySpeed(float plusMinus)
    {
        temporarilySpeed += plusMinus;
    }

    public void MoreEvilGround()
    {
        temporarilySpeed += 0.01f;
        SetColor(Color.black);
    }

    public void EffectPlayerByTouch(PlayerController player)
    {

    }

    public void DownSize(float size)
	{
        if (transform.localScale.y - size < Ground.GetInstance().groundMinimumHeight)
            transform.localScale = new Vector3(
                1.0f,
                Ground.GetInstance().groundMinimumHeight,
                1.0f
                );
        else
            transform.localScale += Vector3.down * size;

    }

    public void SortColliders()
    {
        colliders = SortByY(colliders);
        SetCollidersHeightRange();
    }

    public void BindHeight()
    {
        float height = transform.localScale.y;
        float bindedHeight = Mathf.Clamp(
            height,
            Ground.GetInstance().groundMinimumHeight,
            GetMaxHeightBeforeFirstCollider()
            );
        if (height != bindedHeight)
            SetGroundTransform(bindedHeight);
    }

    public void UpDown()
    {
        if (!temporailyStop)
        {
            float height = transform.localScale.y;
            float heightDestination = GetSafeHeight(GetHeightDestination());
            float upDownY = Mathf.Sign(heightDestination - height) * Mathf.Abs(heightDestination - height) * temporarilySpeed * Time.deltaTime;
            if (heightDestination < height && heightDestination > height + upDownY)
                upDownY = heightDestination - height;
            if (heightDestination > height && heightDestination < height + upDownY)
                upDownY = heightDestination - height;

            SetGroundTransform(height + upDownY);
        }
    }

    public void LiftUpColliders()
    {
        float beforeTopY = transform.position.y + transform.localScale.y * 0.5f;
        float hitY;
        float hitNewY;
        float height = transform.localScale.y;
        float regularLiftY = Mathf.Max(height - GetMaxHeightBeforeFirstCollider(), 0.0f);

        for (int i = 0; i < collidersNumber; ++i)
        {
            Transform hit = colliders[i];
            float hitHeightHalf = Tools.GetInstance().GetHeight(hit) * 0.5f;
            hitY = hit.transform.position.y;

            if (hitY - hitHeightHalf < beforeTopY)
            {
                hitNewY = hitY + regularLiftY;
                if (hitNewY + hitHeightHalf > Ground.GetInstance().groundY1)
                    hitNewY = Ground.GetInstance().groundY1 - hitHeightHalf;

                hit.position = new Vector3(
                    hit.position.x,
                    hitNewY,
                    hit.position.z
                    );

                beforeTopY = hitNewY + hitHeightHalf;

                if (hitNewY != hit.position.y)
                {
                    float diff = hitNewY - hit.transform.position.y;
                    if (hit.name == "Player")
                        Tools.GetInstance().groundLiftMinorDiffrence = diff;
                    //Debug.LogFormat("{0}, {1}, {2}, {3}, {4}, {5}", hit.name, hitNewY, hit.transform.position.y, diff, Tools.GetInstance().groundLiftMinorDiffrence, diff - Tools.GetInstance().groundLiftMinorDiffrence);
                }
            }
            else
                beforeTopY = hitY + hitHeightHalf;
        /*
            Y(colliders[i]);
            */
        }
        
        
        SortColliders();
    }

    public void SqueezePlayer(PlayerController player)
    {
        bool toughSpace = GetEmptySpaceHeight() < 0.01f;
        if (Tools.GetInstance().SameGround(player.transform, transform) && toughSpace)
            player.Squeeze(Time.deltaTime);
    }

    public void Y(Transform other)
    {
        float topY = Tools.GetInstance().GetTopY(transform);
        float halfHeight = Tools.GetInstance().GetHeight(other) * 0.5f;
        float newY = topY + halfHeight;
        float bottomY = newY - halfHeight;
        float diff = topY - bottomY;
        
        other.position = new Vector3(
            other.position.x,
            newY,
            other.position.z
            );

        if (newY != other.position.y)
        {
            float diff2 = newY - other.transform.position.y;
            Debug.LogFormat("{0}, {1}, {2}, {3}", other.name, newY, other.transform.position.y, diff2);
        }
    }

    private float GetSafeHeight(float height)
    {
        return Mathf.Clamp(
            height, 
            Ground.GetInstance().groundMinimumHeight, 
            Ground.GetInstance().groundHeight - GetCollidersUniqueHeight()
            );
    }

    private float GetCollidersUniqueHeight()
    {
        float height = 0.0f;
        for (int i = 0; i < collidersHeightRangeBottom.Count; ++i)
            height += collidersHeightRangeTop[i] - collidersHeightRangeBottom[i];

        return height;
    }

    private float GetEmptySpaceHeight()
    {
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        float emptyHeight = Ground.GetInstance().groundY1 - transform.position.y - transform.localScale.y * 0.5f;
        
        for (int i = 0; i < collidersHeightRangeBottom.Count; ++i)
        {
            float top = Mathf.Max(collidersHeightRangeTop[i], topY);
            float bottom = Mathf.Max(collidersHeightRangeBottom[i], topY);
            emptyHeight -=  (top - bottom);
        }

        return emptyHeight > 0.0f ? emptyHeight : 0.0f;
    }

    private float GetMaxHeightBeforeFirstCollider()
    {
        if (collidersNumber == 0)
            return Ground.GetInstance().groundHeight;

        else
            return Mathf.Max(
                Tools.GetInstance().GetBottomY(colliders[0]) - Ground.GetInstance().groundY0, 
                0.0f
                );

    }

    private float GetHeightDestination()
    {
        return transform.localScale.y + GetUpDownY();
    }

    private float GetUpDownY()
    {
        float upDownY = 0.0f;
        float upDownLimit = 3.0f;
        int i = 0;
        List<Transform> uppers = Status.GetInstance().groundUppers;
        bool notTouch = true;

        while (i < uppers.Count)
        {
            Transform target = uppers[i];

            if (target == null)
                uppers.RemoveAt(i);
            else
            {
                i++;
                if (Tools.GetInstance().SameGround(transform, target))
                {
                    upDownY += target.localScale.y * 2.0f;
                }
                else
                {
                    float distanceXZ = Tools.GetInstance().GetDistanceXZ(transform, target);
                    float targetSpecial = Status.GetInstance().groundUpDownWeakPower;
                    upDownY += targetSpecial * Mathf.Max(
                        3.0f - distanceXZ,
                        -3.0f
                        );
                }
            }
        }

        if (i == 0)
            return -upDownLimit;
        else
            return  Mathf.Clamp(upDownY, -upDownLimit, upDownLimit);
    }

    private void SetGroundTransform(float height)
	{
        if (height > Ground.GetInstance().groundHeight)
            height = Ground.GetInstance().groundHeight;
        else if (height < 1)
            height = 1;

        transform.localScale = new Vector3(
            1.0f,
            height,
            1.0f
            );

        transform.position = new Vector3(
            groundX,
            Ground.GetInstance().groundY0 + height * 0.5f,
            groundZ
            );
    }

    private void SetCollidersHeightRange()
    {
        collidersHeightRangeBottom = new List<float>();
        collidersHeightRangeTop = new List<float>();

        bool change = true;

        for (int i = 0; i < collidersNumber; ++i)
        {
            Transform collider = colliders[i];
            float halfHeight = Tools.GetInstance().GetHeight(collider) * 0.5f;
            float bottom = collider.position.y - halfHeight;
            float top = collider.position.y + halfHeight;
            collidersHeightRangeBottom.Add(bottom);
            collidersHeightRangeTop.Add(top);
        }

        while (change)
        {
            change = false;
            int i = 0;
            while (i < collidersHeightRangeBottom.Count)
            {
                float bottom = collidersHeightRangeBottom[i];
                float top = collidersHeightRangeTop[i];
                bool inRange = false;
                for (int i2 = i + 1; i2 < collidersHeightRangeBottom.Count; ++i2)
                {
                    float bottom2 = collidersHeightRangeBottom[i2];
                    float top2 = collidersHeightRangeTop[i2];

                    if (bottom <= bottom2 && top >= top2)
                    {
                        inRange = true;
                        collidersHeightRangeBottom[i2] = bottom;
                        collidersHeightRangeTop[i2] = top;
                        break;
                    }
                    else if (bottom < bottom2 && top >= bottom2)
                    {
                        inRange = true;
                        collidersHeightRangeBottom[i2] = bottom;
                        collidersHeightRangeTop[i2] = Mathf.Max(top, top2);
                        break;
                    }
                }

                if (inRange)
                {
                    collidersHeightRangeBottom.RemoveAt(i);
                    collidersHeightRangeTop.RemoveAt(i);
                    change = true;
                }
                else
                    i++;
            }

        }
    }

    private void SetColor(Color destColor)
    {
        GetComponent<Renderer>().material.color = destColor;
    }

    private List<Transform> SortByY(List<Transform> hits)
    {
        for (int i = 0; i < collidersNumber; ++i)
        {
            int minI = i;
            for (int i2 = i + 1; i2 < collidersNumber; ++i2)
            {
                float bottomY = Tools.GetInstance().GetBottomY(hits[minI]);
                float bottomY2 = Tools.GetInstance().GetBottomY(hits[i2]);
                
                if (bottomY > bottomY2)
                    minI = i2;
            
            }

            Transform swap = hits[i];
            hits[i] = hits[minI];
            hits[minI] = swap;
        }

        return hits;
    }
}
