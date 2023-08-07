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
    public List<GameObject> colliders;
    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    private const float defaultSpeed = 0.4f;

    //private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        collidersNumber = 0;
        colliders = new List<GameObject>();
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

    public void AddOnBoardCollider(GameObject collider)
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
        colliders = SortByY(colliders);
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
            Ground.GetInstance().groundHeight - GetEmptySpaceHeight()
            );

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

    private bool LiftUpColliders()
    {
        float emptyHeight = GetEmptySpaceHeight();
        float bottomY = transform.position.y + transform.localScale.y * 0.5f;

        float hitHeightHalf;
        foreach (GameObject hit in colliders)
        {
            hitHeightHalf = Tools.GetInstance().GetHeight(hit.transform) * 0.5f;
            
            bool hitNeedLift = false;
            if (hit.transform.name == "Player" && Tools.GetInstance().SameGround(hit.transform, transform))
                hitNeedLift = true;
            else if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Structure")
                hitNeedLift = true;

            float liftY = bottomY - (hit.transform.position.y - hitHeightHalf);
            //bool bug = hit.transform.GetComponent<LivingBall>() && hit.transform.name != "Player";
            if (hitNeedLift && liftY >= 0.0f)
            {
                float validLiftY = Mathf.Min(liftY, emptyHeight);
                emptyHeight -= validLiftY;
                
                hit.transform.position = new Vector3(
                    hit.transform.position.x,
                    bottomY + validLiftY + hitHeightHalf,
                    hit.transform.position.z
                    );
                bottomY = hit.transform.position.y + hitHeightHalf;
                
                if (liftY >= emptyHeight)
                    return true;

            }
        }

        return false;
    }

    private float GetEmptySpaceHeight()
    {
        float emptyHeight = Ground.GetInstance().groundY1 - transform.position.y - transform.localScale.y * 0.5f;
        foreach (GameObject collider in colliders)
            emptyHeight -= Tools.GetInstance().GetHeight(collider.transform);

        return emptyHeight > 0.0f ? emptyHeight : 0.0f;
    }

    private float GetMaxHeightWithCollides()
    {
        float maxHeight = Ground.GetInstance().groundHeight;
        LayerMask groundMask = LayerMask.GetMask("Structure");
        LayerMask playerMask = LayerMask.GetMask("Player");

        RaycastHit[] structureHits = Physics.RaycastAll(
            transform.position, Vector3.up, Mathf.Infinity, groundMask
            );

        RaycastHit playerHit;

        if (Physics.Raycast(transform.position, Vector3.up, out playerHit, Mathf.Infinity, playerMask))
        {
            if (playerHit.transform.name == "Player")
                maxHeight -= playerHit.transform.localScale.x;
        }
            
        maxHeight -= structureHits.Length;

        return maxHeight;
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
        
        if (distance > 3.0f)
            return Ground.GetInstance().groundMinimumHeight;
        else
            return Mathf.Max(
                Ground.GetInstance().groundHeight - distance,
                Ground.GetInstance().groundMinimumHeight
                );
    }

    private float GetSafeHeight(float height)
    {
        return Mathf.Clamp(
            height, 
            Ground.GetInstance().groundMinimumHeight, 
            GetMaxHeightWithCollides()
            );
    }

    private void SetColor(Color destColor)
    {
        GetComponent<Renderer>().material.color = destColor;
    }

    private List<GameObject> SortByY(List<GameObject> hits)
    {
        for (int i = 0; i < collidersNumber - 1; ++i)
        {
            int minI = i;
            for (int i2 = i + 1; i2 < hits.Count; ++i2)
            {
                if (hits[minI].transform.position.y > hits[i2].transform.position.y)
                {
                    minI = i2;
                }
            }

            GameObject swap = hits[i];
            hits[i] = hits[minI];
            hits[minI] = swap;
        }

        return hits;
    }
}
