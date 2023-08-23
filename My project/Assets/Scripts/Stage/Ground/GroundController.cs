using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;
    public int collidersNumber;
    public bool temporailyStop;
    public float heightDestination;
    public List<float> collidersHeightRangeBottom = new List<float>();
    public List<float> collidersHeightRangeTop = new List<float>();

    private const float defaultSpeed = 0.2f;
    private const float evilSpeed = 0.5f;
    private const string startName = "startName";
    private const string neutralName = "neutral";
    private string ownerName;
    private float colorDensitySum = 1.0f;
    private float colorDensityMaxSum;
    private float groundX;
    private float groundZ;
    private float temporailySpeed;
    private List<Transform> colliders;
    private Color destColor;
    private Color startColor;
    private Color baseColor = Color.white;
    
    private void Awake()
    {
        temporailySpeed = defaultSpeed;
        collidersNumber = 0;
        colliders = new List<Transform>();
        temporailyStop = false;
        ownerName = startName;
        startColor = GetComponent<Renderer>().material.color;
        heightDestination = 0.0f;
    }

    private void Start()
    {
        colorDensityMaxSum = Ground.GetInstance().groundHeight;
    }

    public void BeforeCycle()
    {
        temporailySpeed = defaultSpeed;
        SetDestColorByOwner();
        ColorByDensity();
        collidersNumber = 0;
        temporailyStop = false;

        temporailySpeed = defaultSpeed;
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

    public void SetGroundtemporailySpeed(float plusMinus)
    {
        temporailySpeed += plusMinus;
    }

    public void SetEvilGround()
    {
        temporailySpeed = evilSpeed;
        SetColor(Color.black);
    }

    public void PaintColor(string attackOwner, float paintDensity)
    {
        if (ownerName == attackOwner)
            colorDensitySum += paintDensity;
        else
            colorDensitySum -= paintDensity;

        if (colorDensitySum == 0.0f)
        {
            ownerName = neutralName;
            SetDestColorByOwner();
        }
        else if (colorDensitySum < 0.0f)
        {
            ownerName = attackOwner;
            SetDestColorByOwner();
            colorDensitySum = -colorDensitySum;
        }
        else if (colorDensitySum > colorDensityMaxSum)
        {
            colorDensitySum = colorDensityMaxSum;
        }
    }

    public void AttackGround(float size)
	{
        if (transform.localScale.y - size < Ground.GetInstance().groundMinimumHeight)
            transform.localScale = new Vector3(
                1.0f,
                Ground.GetInstance().groundMinimumHeight,
                1.0f
                );
        else
        {
            transform.localScale += Vector3.down * size;
        }

    }

    public void SortColliders()
    {
        colliders = SortByBottomY(colliders);
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
            SetGroundTransform(GetHeightDestination());
    
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
            }
            else
                beforeTopY = hitY + hitHeightHalf;
        }
        
        SortColliders();
    }

    public void SqueezePlayer(PlayerController player)
    {
        if (NeedSqueeze() && Tools.GetInstance().SameGround(player.transform, transform))
            player.Squeeze(Time.deltaTime);
    }

    public bool NeedSqueeze()
    {
        return GetEmptyHeight() < 0.01f;
    }

    public float GetEmptyHeight()
    {
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        float emptyHeight = Ground.GetInstance().groundY1 - transform.position.y - transform.localScale.y * 0.5f;

        for (int i = 0; i < collidersHeightRangeBottom.Count; ++i)
        {
            float top = Mathf.Max(collidersHeightRangeTop[i], topY);
            float bottom = Mathf.Max(collidersHeightRangeBottom[i], topY);
            emptyHeight -= (top - bottom);
        }

        return emptyHeight > 0.0f ? emptyHeight : 0.0f;
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

    private void SetDestColorByOwner()
    {
        destColor = GetOwnerNameColor(ownerName);
    }

    private void ColorByDensity()
    {
        Color newColor;
        newColor = Tools.GetInstance().LerpColor(baseColor, destColor, GetDensity());
        GetComponent<Renderer>().material.color = newColor;
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
        return GetSafeHeight(
            transform.localScale.y + GetUpDownY() * Time.deltaTime * temporailySpeed
            );
    }

    private float GetUpDownY()
    {
        float upDownY = -1.0f;
        float upDownLimit = 3.0f;
        int i = 0;
        List<Transform> uppers = Status.GetInstance().groundUppers;

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
                    upDownY += 2.6f;
                }
                else
                {
                    float distanceXZ = Tools.GetInstance().GetDistanceXZ(transform, target);
                    
                    if (distanceXZ < 4.0f)
                        upDownY += Mathf.Max(3.0f - distanceXZ, -1.0f);

                }
            }
        }

        return  Mathf.Clamp(upDownY, -upDownLimit, upDownLimit);
    }

    private float GetDensity()
    {
        float height = transform.localScale.y;
        if (height == 0.0f)
            return 0.0f;
        else
            return Mathf.Clamp01(colorDensitySum / height);
    
    }

    private List<Transform> SortByBottomY(List<Transform> hits)
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

    private Color GetOwnerNameColor(string ownerName)
    {
        Color color = baseColor;
        if (ownerName == neutralName)
            color = baseColor;
        else if (ownerName == startName)
            color = startColor;
        else if (ownerName == Status.GetInstance().enemyName)
            color = Status.GetInstance().enemyColor;
        else if (ownerName == Status.GetInstance().playerName)
            color = Status.GetInstance().playerColor;

        return color;
    }
}
