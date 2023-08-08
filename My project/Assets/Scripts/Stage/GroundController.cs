using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    private Color originalColor;
    public int collidersNumber;
    public List<Transform> colliders;
    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    private const float defaultSpeed = 0.4f;

    //private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        collidersNumber = 0;
        colliders = new List<Transform>();
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

    public void LiftUpOrSqueeze(PlayerController player)
    {
        bool toughSpace = LiftUpColliders();

        if (Tools.GetInstance().SameGround(player.transform, transform) && toughSpace)
        {
            player.Squeeze(Time.deltaTime);
        }
    }

    public void BindHeight()
    {
        float height = transform.localScale.y;
        float bindedHeight = Mathf.Clamp(
            height,
            Ground.GetInstance().groundMinimumHeight,
            GetMaxHeightWithLiftedCollides()
            ); ;

        if (height != bindedHeight)
            SetGroundTransform(bindedHeight);
    }

    public void UpDown(PlayerController player)
    {
        float height = transform.localScale.y;
        float heightDestination = GetSafeHeight(GetHeightDestination(player.transform));
        float upDownY = Mathf.Sign(heightDestination - height) * MathF.Abs(heightDestination - height) * temporarilySpeed * Time.deltaTime;
        if (heightDestination < height && heightDestination > height + upDownY)
            upDownY = heightDestination - height;
        if (heightDestination > height && heightDestination < height + upDownY)
            upDownY = heightDestination - height;

        SetGroundTransform(height + upDownY);
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
    }

    private bool LiftUpColliders()
    {
        float emptyHeight = GetEmptySpaceHeight();
        float beforeTopY = transform.position.y + transform.localScale.y * 0.5f;
        float hitY;
        float hitNewY;
        foreach (Transform hit in colliders)
        {
            float hitHeightHalf = Tools.GetInstance().GetHeight(hit) * 0.5f;
            hitY = hit.transform.position.y;

            bool needLift = hitY - hitHeightHalf < beforeTopY;

            hitNewY = beforeTopY + hitHeightHalf;
            if (hitNewY > Ground.GetInstance().groundY1)
            {
                hitNewY = Ground.GetInstance().groundY1 - hitHeightHalf;
            }

            if (needLift)
            {
                emptyHeight -= beforeTopY - (hitNewY + hitHeightHalf);
                
                hit.position = new Vector3(
                    hit.position.x,
                    hitNewY,
                    hit.position.z
                    );
                beforeTopY = hitNewY + hitHeightHalf;
                
            }
        }

        return emptyHeight == 0.0f;
    }

    private float GetSafeHeight(float height)
    {
        return Mathf.Clamp(
            height, 
            Ground.GetInstance().groundMinimumHeight, 
            GetMaxHeightWithLiftedCollides()
            );
    }

    private float GetCollidersHeightSum()
    {
        float colliderHeight = 0.0f;
        foreach (Transform collider in colliders)
            colliderHeight += Tools.GetInstance().GetHeight(collider);
        return colliderHeight;
    }

    private float GetEmptySpaceHeight()
    {
        float emptyHeight = Ground.GetInstance().groundY1 - transform.position.y - transform.localScale.y * 0.5f;
        /*
        List<float> rangeLeft = new List<float>();
        List<float> rangeRight = new List<float>();
        for (int i = 0; i < collidersNumber; ++i)
        {
            Transform collider = colliders[i];
            float halfHeight = Tools.GetInstance().GetHeight(collider) * 0.5f;
            bool inRange = false;
            float left = collider.position.y - halfHeight;
            float right = collider.position.y + halfHeight;
            for (int i2 = 0; i2<rangeLeft.Count; ++i2)
            {
                if (left < rangeLeft[i2] && right > rangeRight[i2])
                {
                    rangeLeft[i2] = left;
                    rangeRight[i2] = right;
                }
                else if (left < rangeLeft[i2] && right < rangeRight[i2])
                    rangeLeft[i2] = left;
                else if (left > rangeLeft[i2] && right > rangeRight[i2])
                    rangeRight[i2] = right;
            }

            if (!inRange)
            {
                rangeLeft.Add(collider.position.y - halfHeight);
                rangeRight.Add(collider.position.y + halfHeight);
            }
        }
        */
        emptyHeight -= GetCollidersHeightSum();

        return emptyHeight > 0.0f ? emptyHeight : 0.0f;
    }

    private float GetMaxHeightWithLiftedCollides()
    {
        // colliders 전부를 위로 밀어냈을 때를 가정.
        return Ground.GetInstance().groundHeight - GetCollidersHeightSum();
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

    private float GetHeightDestination(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        float distance = distVector.magnitude;

        /*
        return GetMaxHeightWithLiftedCollides();
        */
        if (Tools.GetInstance().SameGround(player, transform))
            return GetMaxHeightWithLiftedCollides();
        else if (distance < 3.0f)
            return Ground.GetInstance().groundHeight - distance;
        else
            return Ground.GetInstance().groundMinimumHeight;
    }

    private void SetColor(Color destColor)
    {
        GetComponent<Renderer>().material.color = destColor;
    }

    private List<Transform> SortByY(List<Transform> hits)
    {
        for (int i = 0; i < collidersNumber - 1; ++i)
        {
            int minI = i;
            for (int i2 = i + 1; i2 < hits.Count; ++i2)
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
