using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeController : MonoBehaviour
{
	private const float followSpeed = 4.0f;

	// body.eulerAngles °ª ¹üÀ§ (-180.0f, 180.0f)
    public void FollowTarget(Transform body)
	{
		Vector3 diffPosition = body.position - transform.position;
		Vector3 diffEulerAngles = body.eulerAngles - transform.eulerAngles;
		Vector3 diffLocalScale = body.localScale - transform.localScale;

		float t = Mathf.Clamp01(followSpeed * Time.deltaTime);
		
		float diffDegX, diffDegY, diffDegZ;
		diffDegX = diffEulerAngles.x;
		diffDegY = diffEulerAngles.y;
		diffDegZ = diffEulerAngles.z;
		if (Mathf.Abs(diffDegX) > 180.0f)
			diffDegX = (Mathf.Abs(diffDegX) - 360.0f) * Mathf.Sign(diffDegX);
		if (Mathf.Abs(diffDegY) > 180.0f)
			diffDegY = (Mathf.Abs(diffDegY) - 360.0f) * Mathf.Sign(diffDegY);
		if (Mathf.Abs(diffDegZ) > 180.0f)
			diffDegZ = (Mathf.Abs(diffDegZ) - 360.0f) * Mathf.Sign(diffDegZ);
		diffEulerAngles = new Vector3(diffDegX, diffDegY, diffDegZ);
		
		transform.position += diffPosition * t;
		transform.eulerAngles += diffEulerAngles * t;
		transform.localScale += diffLocalScale * t;
	}
}
