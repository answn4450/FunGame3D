using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    public List<Transform> colliders;
    public int collidersNumber;
    public bool temporailyStop;
    
    private Color originalColor;
    public List<float> collidersHeightRangeBottom = new List<float>();
    public List<float> collidersHeightRangeTop = new List<float>();
    private const float defaultSpeed = 0.4f;
    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    //private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        collidersNumber = 0;
        colliders = new List<Transform>();
        temporailyStop = false;
        //slowdownPlayer = false;
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void Reset()
    {
        temporarilySpeed = defaultSpeed;
        SetColor(originalColor);
    }

    public void ResetOnBoardColliders()
    {
        collidersNumber = 0;
    }

    public void AddOnBoardCollider(Transform collider)
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

    public void UpDown(PlayerController player)
    {
        if (!temporailyStop)
        {
            float height = transform.localScale.y;
            float heightDestination = GetSafeHeight(GetHeightDestination(player.transform));
            float upDownY = Mathf.Sign(heightDestination - height) * Mathf.Abs(heightDestination - height) * temporarilySpeed * Time.deltaTime;
            if (heightDestination < height && heightDestination > height + upDownY)
                upDownY = heightDestination - height;
            if (heightDestination > height && heightDestination < height + upDownY)
                upDownY = heightDestination - height;

            SetGroundTransform(height + upDownY * Time.deltaTime);
        }
    }

    public void LiftUpOrSqueeze(PlayerController player)
    {
        LiftUpColliders();
        bool toughSpace = GetEmptySpaceHeight() < 0.01f;

        if (Tools.GetInstance().SameGround(player.transform, transform))
            Debug.Log(GetEmptySpaceHeight());
        if (Tools.GetInstance().SameGround(player.transform, transform) && toughSpace)
            Debug.LogError("squeeze");
        //    player.Squeeze(Time.deltaTime);
    }

    private void LiftUpColliders()
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
        
    }

    public void Y(Transform other)
    {
        float newY = Tools.GetInstance().GetTopY(transform) + Tools.GetInstance().GetHeight(other) * 0.5f;
        
        other.position = new Vector3(
            other.position.x,
            newY,
            other.position.z
            );
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

    private float GetHeightDestination(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        float distance = distVector.magnitude;

        if (Tools.GetInstance().SameGround(player, transform))
            return Ground.GetInstance().groundHeight - GetCollidersUniqueHeight();
        else if (distance < 3.0f)
            return Ground.GetInstance().groundHeight - distance;
        else
            return Ground.GetInstance().groundMinimumHeight;
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
                if (hits[minI].position.y > hits[i2].position.y)
                {
                    minI = i2;
                }
            }

            Transform swap = hits[i];
            hits[i] = hits[minI];
            hits[minI] = swap;
        }

        return hits;
    }
}
