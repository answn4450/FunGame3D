using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private float baseY0;
	private float Length;

    private PlayerController Player;
	private Vector3 ShakePower;

	private void Awake()
	{
		baseY0 = 2.0f;
		Length = 14.0f;
	}


	public void BehindPlayer(float deg)
	{
		transform.localPosition = new Vector3(
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
		transform.localPosition = new Vector3(
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
			ShakePower += new Vector3(
				Random.Range(-power, power),
				Random.Range(-power, power),
				0.0f
				);
        }
    }
}
