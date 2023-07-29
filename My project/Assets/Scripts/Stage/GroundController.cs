using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;
    private float groundX;
    private float groundZ;
    private float speed;
    private float distCheck;

    private void Awake()
    {
        speed = 0.8f;
        distCheck = 2.0f;
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

    public void UpDownOrSqueeze(PlayerController player)
	{
        float power = speed * Time.deltaTime;
        float newHeight = transform.localScale.y + UpDownByTarget(player.transform) * power;
        float radius = player.transform.localScale.y * 0.5f;

        if (Tools.GetInstance().SameGround(player.transform, transform))
		{
            float maxHeight = Ground.GetInstance().groundHeight - radius * 2.0f;
            if (newHeight > maxHeight)
			{
                player.Hurt((newHeight - maxHeight) * Time.deltaTime);
                newHeight = maxHeight;
			}

            if (player.transform.position.y - radius < Ground.GetInstance().groundY0 + newHeight)
                player.transform.position = new Vector3(
                    player.transform.position.x,
                    Ground.GetInstance().groundY0 + newHeight + radius,
                    player.transform.position.z
                    );
        }

        SetSize(newHeight);
    }

    public void DownSize(float size)
	{
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        if (topY - size < 0.0f)
            size = topY;

        transform.localScale += Vector3.down * size;
    }

    public float GetTopY()
	{
        return transform.position.y + transform.localScale.y * 0.5f; ;
    }

    private float UpDownByTarget(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        float changeY = distCheck - distVector.magnitude;
        float availableY = Ground.GetInstance().groundHeight - transform.localScale.y;
        if (changeY > availableY)
            return availableY; 
        if (changeY + transform.localScale.y < 1.0f)
            return (1.0f - transform.localScale.y);
        else
            return changeY;
    }

    private void SetSizePlusMinus(float plusMinus)
    {
        float height = transform.localScale.y + plusMinus;
        SetSize(height);
    }

    private void SetSize(float height)
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
}
