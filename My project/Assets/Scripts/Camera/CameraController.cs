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

	private Vector3 anchorPosition, velocity;

	private const float basicFieldViewDegree = 50.0f;
	private float baseY0;
	private float Length;
	private PlayerController Player;
	private float swivelDegZ;
	private float fieldViewDegree;

	private void Awake()
	{
		baseY0 = 2.0f;
		Length = 14.0f;
		velocity = Vector3.zero;
		fieldViewDegree = basicFieldViewDegree;
	}

	private void Start()
	{
		BehindPlayer(10.0f);
		transform.localPosition = anchorPosition;
	}

	private void Update()
	{
		BindTransform();
	}

	private void LateUpdate()
	{
		Vector3 displacement = anchorPosition - transform.localPosition;
		Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
		velocity += acceleration * Time.deltaTime;
		transform.localPosition += velocity * Time.deltaTime;

		transform.rotation = Quaternion.Euler(
			transform.eulerAngles.x,
			transform.eulerAngles.y,
			swivelDegZ
			);

		GetComponent<Camera>().fieldOfView = fieldViewDegree;
	}

	private void BindTransform()
	{
		swivelDegZ = Mathf.Lerp(swivelDegZ, 0.0f, Time.deltaTime);
		fieldViewDegree = Mathf.Lerp(fieldViewDegree, basicFieldViewDegree, Time.deltaTime);
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

	public void SwivelZ(float t)
	{
		if (Mathf.Abs(swivelDegZ) < 20.0f)
		{
			swivelDegZ -= Time.deltaTime * t * 2;
		}
		else if (Mathf.Abs(swivelDegZ - t) < Mathf.Abs(swivelDegZ))
			swivelDegZ -= Time.deltaTime * t * 5;
	}

	public void ChangeFieldView(float t)
	{
		if (Mathf.Abs(fieldViewDegree + Time.deltaTime * t * 10.0f - basicFieldViewDegree) <20.0f)
			fieldViewDegree += Time.deltaTime * t * 10.0f;
	}
}