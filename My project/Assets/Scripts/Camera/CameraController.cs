using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private float baseY0;
	private float Length;

	private GameObject target;
    private PlayerController Player;

	private void Awake()
	{
		baseY0 = 2.0f;
		Length = 14.0f;
	}

	private void Start()
	{
		target = GameObject.Find("Player");
		Player = target.GetComponent<PlayerController>();
	}

	void Update()
	{
		if (true)
		{
		}
		else
		{
			SetTransform(10.0f);
		}
	}

	private void SetTransform(float deg)
	{
		transform.position = new Vector3(
			target.transform.position.x,
			baseY0 + Mathf.Sin(Mathf.Deg2Rad * deg) * Length + target.transform.position.y,
			Mathf.Cos(Mathf.Deg2Rad * deg) * Length + target.transform.position.z
			);

		Vector3 plrDeg = target.transform.rotation.eulerAngles;
		transform.localRotation = Quaternion.Euler(
			deg + plrDeg.x, 
			plrDeg.y,
			plrDeg.z
			);
	}

	public void SetTransformByY(float deg)
	{
		transform.localPosition = new Vector3(
			Mathf.Cos(Mathf.Deg2Rad * deg) * Length,
			baseY0,
			Mathf.Sin(Mathf.Deg2Rad * deg) * Length
			);

		transform.localRotation = Quaternion.Euler(0.0f, deg, 0.0f);
	}
}
