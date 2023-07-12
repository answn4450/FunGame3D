using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

	[SerializeField, Min(0f)]
	float
		springStrength = 100f,
		dampingStrength = 10f,
		jostleStrength = 40f,
		pushStrength = 1f;

	private Vector3 anchorPosition, velocity, test;

	private float baseY0;
	private float Length;
	private PlayerController Player;
	private Vector3 shakePower;

	private void Awake()
	{
		baseY0 = 2.0f;
		Length = 14.0f;
		anchorPosition = transform.localPosition;
		velocity = Vector3.zero;
	}

    private void LateUpdate()
    {
		Vector3 displacement = anchorPosition - transform.localPosition;
		Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
		velocity += acceleration * Time.deltaTime;
		transform.localPosition += velocity * Time.deltaTime;
	}

	public void PushXY(Vector2 impulse)
	{
		velocity.x += pushStrength * impulse.x;
		velocity.y += pushStrength * impulse.y;
	}

	public void BehindPlayer(float deg)
	{
		anchorPosition = new Vector3(
			0.0f,
			baseY0 + Mathf.Sin(Mathf.Deg2Rad * deg) * Length,
			-Mathf.Cos(Mathf.Deg2Rad * deg) * Length
			);

		transform.localRotation = Quaternion.Euler(
			deg, 
			0.0f,
			0.0f
			);
	}

	public void AroundPoint(float deg)
	{
		anchorPosition = new Vector3(
			Mathf.Cos(Mathf.Deg2Rad * deg) * Length,
			baseY0,
			Mathf.Sin(Mathf.Deg2Rad * deg) * Length
			);

		transform.localRotation = Quaternion.Euler(0.0f, -(deg + 90.0f), 0.0f);
	}

	public IEnumerator Shake(float power)
    {
		if (power > 0.01)
        {
			yield return null;
			power -= Time.deltaTime;
			shakePower += new Vector3(
				Random.Range(-power, power),
				Random.Range(-power, power),
				0.0f
				);
        }
    }
}
