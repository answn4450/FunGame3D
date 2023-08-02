using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    private GameObject squeezeFX;

    private Color originalColor;
    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    private const float defaultSpeed = 0.4f;

    private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        squeezeFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        slowdownPlayer = false;
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void Reset()
    {
        temporarilySpeed = defaultSpeed;
        SetColor(originalColor);
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
        slowdownPlayer = true;
    }

    public void ResetTemporarilyOptions()
    {
        temporarilySpeed = defaultSpeed;
        slowdownPlayer = false;
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
        float playerRadius = player.transform.localScale.y * 0.5f;

        float maxHeightWithCollides = GetMaxHeightWithCollides();
        if (Tools.GetInstance().SameGround(player.transform, transform))
        {
            if (newHeight > maxHeightWithCollides)
            {
                Instantiate(squeezeFX).transform.position = player.transform.position;
                player.Hurt((newHeight - maxHeightWithCollides) * Time.deltaTime);
                newHeight = maxHeightWithCollides;
                Debug.Log("d");
            }

            if (player.transform.position.y - playerRadius < Ground.GetInstance().groundY0 + newHeight)
                player.transform.position = new Vector3(
                    player.transform.position.x,
                    Ground.GetInstance().groundY0 + newHeight + playerRadius,
                    player.transform.position.z
                    );
        }

        SetGroundTransform(newHeight);
    }

    public void EffectPlayerByTouch(PlayerController player)
    {

    }

    public void DownSize(float size)
	{
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        if (topY - size < 0.0f)
            size = topY;

        transform.localScale += Vector3.down * size;
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
            Ground.GetInstance().groundHeight
            );
    }

    private void SetColor(Color destColor)
    {
        GetComponent<Renderer>().material.color = destColor;
    }
}
