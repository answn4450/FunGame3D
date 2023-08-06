using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    private Color originalColor;
    private int collidersNumber;
    private List<GameObject> colliders;
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
        colliders = new List<GameObject>();
    }

    public void AddOnBoardCollider(GameObject collider)
    {
        colliders.Add(collider);
        //colliders[collidersNumber]=collider;
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
        temporarilySpeed += 2.0f;
        SetColor(Color.black);
    }

    public void UpDownOrSqueeze(PlayerController player)
    {
        float newHeight = GetSafeHeight(
            transform.localScale.y + GetUpDownMove(player.transform)
            );
        SetGroundTransform(newHeight);

        float upDownY = newHeight - transform.localScale.y;
        if (upDownY >= 0.0f)
        {
            bool toughSpace = LiftUpCollides();

            if (Tools.GetInstance().SameGround(player.transform, transform) && toughSpace)
            {
                player.Squeeze(Time.deltaTime);
            }
        }

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

    private bool LiftUpCollides()
    {
        float emptyHeight = GetEmptySpaceHeight();
        List<GameObject> sortedHits = SortByY(colliders);
        float bottomY = transform.position.y + transform.localScale.y * 0.5f;

        float hitHeightHalf;
        foreach (GameObject hit in sortedHits)
        {
            if (hit.transform.name == "Player")
                hitHeightHalf = hit.transform.localScale.x * 0.5f;
            else
                hitHeightHalf = 0.5f;

            bool hitNeedLift = false;
            if (hit.transform.name == "Player" && Tools.GetInstance().SameGround(hit.transform, transform))
                hitNeedLift = true;
            else if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Structure")
                hitNeedLift = true;

            float liftY = bottomY - (hit.transform.position.y - hitHeightHalf);

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
        LayerMask groundMask = LayerMask.GetMask("Structure");
        LayerMask playerMask = LayerMask.GetMask("Player");

        RaycastHit[] structureHits = Physics.RaycastAll(
            transform.position, Vector3.up, Mathf.Infinity, groundMask
            );

        RaycastHit playerHit;

        if (Physics.Raycast(transform.position, Vector3.up, out playerHit, Mathf.Infinity, playerMask))
        {
            if (playerHit.transform.name == "Player")
                emptyHeight -= playerHit.transform.localScale.x;
        }

        emptyHeight -= structureHits.Length;

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

    private float GetUpDownMove(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        float upDown = 3.0f - distVector.magnitude;
        return Mathf.Max(upDown, -1.0f) * temporarilySpeed * Time.deltaTime;
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
