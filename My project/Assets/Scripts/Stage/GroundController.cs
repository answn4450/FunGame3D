using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;
    private float speed;
    private float distCheck;

    private void Awake()
    {
        speed = 0.1f;
        distCheck = 2.0f;
    }

    public void UpDownOrSqueeze(PlayerController player)
    {
        float changeY = UpDownByTarget(player.transform) * speed * Time.deltaTime;
         
        float radius = player.transform.localScale.x * 0.5f;
        float topY = transform.position.y + transform.localScale.y * 0.5f;
        float maxWorldY = topY - transform.localScale.y + maxSizeY;
        float liftPlayerY = topY - player.transform.position.y + radius;
        float emptySpaceY = Mathf.Max(maxWorldY - topY - player.size * 2.0f, 0.0f);

        bool playerAtDown;

        playerAtDown = Tools.GetInstance().SameGround(player.transform, transform);
        playerAtDown = playerAtDown && player.transform.position.y - radius < topY;
        
        if (playerAtDown)
        {
            bool needSqueeze;
            if (needSqueeze = changeY > emptySpaceY)
                changeY = emptySpaceY;
            
            liftPlayerY += changeY;
            player.transform.position += Vector3.up * liftPlayerY;
            
            //if (needSqueeze)
            //    player.Hurt();
        }
        
        SetSizePlusMinus(changeY);

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
        float topY = transform.localScale.y + transform.position.y;
        if (topY + changeY < Status.GetInstance().groundY)
            return Status.GetInstance().groundY - topY; 
        if (topY + changeY > maxSizeY)
            return maxSizeY - transform.localScale.y;
        else
            return changeY;
    }

    private void SetSizePlusMinus(float plusMinus)
    {
        if (transform.localScale.y + 0.5f + plusMinus > maxSizeY)
            plusMinus = maxSizeY - transform.localScale.y - 0.5f;
        if (transform.localScale.y + plusMinus < 1.0f)
            plusMinus = 1.0f - transform.localScale.y;

        Vector3 movement = Vector3.up * plusMinus;
        transform.localScale += movement;
        transform.position += movement * 0.5f;
    }
}
