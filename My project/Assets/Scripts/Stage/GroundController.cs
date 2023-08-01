using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public int maxSizeY;

    private GameObject squeezeFX;

    private float groundX;
    private float groundZ;
    private float temporarilySpeed;
    private const float defaultSpeed = 0.8f;

    private bool slowdownPlayer;

    private void Awake()
    {
        temporarilySpeed = defaultSpeed;
        squeezeFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
        slowdownPlayer = false;
    }

    public void Reset()
    {
        temporarilySpeed = defaultSpeed;
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
        MakeColorCloseTo(Color.black);
    }

    public void UpDownOrSqueeze(PlayerController player)
    {
        float distancePower = GetUpDownSpeed(player.transform);
        float newHeight = GetSafeHeight(
            transform.localScale.y + distancePower * temporarilySpeed * Time.deltaTime
            );
        float playerRadius = player.transform.localScale.y * 0.5f;

        float maxHeightUnderPlayer = Ground.GetInstance().groundHeight - playerRadius * 2.0f;
        if (Tools.GetInstance().SameGround(player.transform, transform))
        {
            if (newHeight > maxHeightUnderPlayer)
            {
                Instantiate(squeezeFX).transform.position = player.transform.position;
                player.Hurt((newHeight - maxHeightUnderPlayer) * Time.deltaTime);
                newHeight = maxHeightUnderPlayer;
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

    public float GetTopY()
	{
        return transform.position.y + transform.localScale.y * 0.5f; ;
    }

    private float GetUpDownSpeed(Transform player)
    {
        Vector3 distVector = player.position - transform.position;
        distVector.y = 0;
        float changeY = 6.0f - distVector.magnitude;
        float availableY = Ground.GetInstance().groundHeight - transform.localScale.y;
        if (changeY > availableY)
            return availableY;
        if (changeY + transform.localScale.y < 1.0f)
            return (1.0f - transform.localScale.y);
        else
            return changeY;
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
        return Mathf.Clamp(height, 0, Ground.GetInstance().groundHeight);
    }

    private void MakeColorCloseTo(Color destColor)
    {
        GetComponent<Renderer>().material.color = LerpColor(
            GetComponent<Renderer>().material.color,
            destColor,
            Time.deltaTime
            );
    }

    private Color LerpColor(Color startColor, Color destColor, float t)
    {
        return new Color(
            Mathf.Lerp(startColor.r, destColor.r, t),
            Mathf.Lerp(startColor.g, destColor.g, t),
            Mathf.Lerp(startColor.b, destColor.b, t)
            );
    }
}
