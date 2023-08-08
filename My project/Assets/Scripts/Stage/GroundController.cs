using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void BindHeight()
    {
        float height = transform.localScale.y;
        float bindedHeight = Mathf.Clamp(
            height,
            Ground.GetInstance().groundMinimumHeight,
            GetMaxHeightBeforeFirstCollider()
            ); ;

        if (height != bindedHeight)
            SetGroundTransform(bindedHeight);
    }

    public void UpDown(PlayerController player)
    {
        float height = transform.localScale.y;
        float heightDestination = GetSafeHeight(GetHeightDestination(player.transform));
        float upDownY = Mathf.Sign(heightDestination - height) * Mathf.Abs(heightDestination - height) * temporarilySpeed * Time.deltaTime;
        if (heightDestination < height && heightDestination > height + upDownY)
            upDownY = heightDestination - height;
        if (heightDestination > height && heightDestination < height + upDownY)
            upDownY = heightDestination - height;

        SetGroundTransform(height + upDownY);
    }

    public void LiftUpOrSqueeze(PlayerController player)
    {
        bool toughSpace = LiftUpColliders();

        if (Tools.GetInstance().SameGround(player.transform, transform) && toughSpace)
        {
            player.Squeeze(Time.deltaTime);
        }
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
            GetMaxHeightBeforeFirstCollider()
            );
    }

    private float GetEmptySpaceHeight()
    {
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        float emptyHeight = Ground.GetInstance().groundY1 - transform.position.y - transform.localScale.y * 0.5f;
        List<float> rangeBottom = new List<float>();
        List<float> rangeTop = new List<float>();
        bool change = true;
        bool debug = transform.name == "ground 7 6";

        for (int i = 0; i < collidersNumber; ++i)
        {
            Transform collider = colliders[i];
            float halfHeight = Tools.GetInstance().GetHeight(collider) * 0.5f;
            float bottom = collider.position.y - halfHeight;
            float top = collider.position.y + halfHeight;
            rangeBottom.Add(bottom);
            rangeTop.Add(top);
        }

        while (change)
        {
            change = false;
            int i = 0;
            while (i < rangeBottom.Count)
            {
                float bottom = rangeBottom[i];
                float top = rangeTop[i];
                bool inRange = false;
                for (int i2 = i; i2< rangeBottom.Count;++i2)
                {
                    float bottom2 = rangeBottom[i2];
                    float top2 = rangeTop[i2];
                    if (bottom < bottom2 && top > top2)
                    {
                        inRange = true;
                        rangeBottom[i2] = bottom;
                        rangeTop[i2] = top;
                        break;
                    }
                    else if (bottom < bottom2 && top < top2)
                    {
                        inRange = true;
                        rangeBottom[i2] = bottom;
                        break;
                    }
                    else if (bottom > bottom2 && top > top2)
                    {
                        inRange = true;
                        rangeTop[i2] = top;
                        break;
                    }
                }

                if (inRange)
                {
                    rangeBottom.RemoveAt(i);
                    rangeTop.RemoveAt(i);
                    change = true;
                }
                else
                    i++;
            }

        }

        for (int i = 0; i < rangeBottom.Count; ++i)
        {
            float top = Mathf.Max(rangeTop[i], topY);
            float bottom = Mathf.Max(rangeBottom[i], topY);
            emptyHeight -=  (top - bottom);
        }

        if (debug)
            Debug.Log(rangeTop[0]-rangeBottom[0]);

        return emptyHeight > 0.0f ? emptyHeight : 0.0f;
    }

    private float GetMaxHeightBeforeFirstCollider()
    {
        float maxHeight = Ground.GetInstance().groundHeight;
        if (collidersNumber == 0)
            return maxHeight;
        else
            return Mathf.Max(Tools.GetInstance().GetBottomY(colliders[0]), 0.0f);

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
        
        return GetMaxHeightBeforeFirstCollider();
        /*
        if (Tools.GetInstance().SameGround(player, transform))
            return GetMaxHeightWithLiftedCollides();
        else if (distance < 3.0f)
            return Ground.GetInstance().groundHeight - distance;
        else
            return Ground.GetInstance().groundMinimumHeight;
        */
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
