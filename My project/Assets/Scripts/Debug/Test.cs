using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Test : MonoBehaviour
{
    public Transform a1;
    public Transform a2;
    public Transform axis1;
    public Transform axis2;
    public Transform b1;
    public Transform b2;

	public List<Renderer> objectRenderers = new List<Renderer>();
	private const string path = "Legacy Shaders/Transparent/Specular";

	private void Update()
    {
		TransparentBlocks();
        //test();
            //a1.position = Vector3.Reflect(a2.position, Vector3.right);
    }

    private void test()
	{
        Vector3 dir = a2.position - a1.position;
        Vector3 axis = axis2.position - axis1.position;
        Vector3 inNormal = Quaternion.Euler(0.0f, 0.0f, 90.0f) * axis;
        Vector3 dir2 = Vector3.Reflect(dir.normalized, inNormal.normalized);
        b2.position = b1.position + dir2 * 3;
        //b2.position = b1.position + inNormal.normalized * 2;
    }

	private void TransparentBlocks()
	{
		List<RaycastHit> hits = new List<RaycastHit>();
		List<Renderer> renderers = new List<Renderer>();

		Vector3 direction = transform.parent.position - transform.position;
		Debug.DrawLine(transform.position, transform.parent.position);
		// ** 모든 충돌을 감지.
		hits = Physics.RaycastAll(transform.position, direction, direction.magnitude).ToList();
		
		// ** 충돌된 모든 원소들 중에 Renderer만 추출한 새로운 리스트를 생성.
		renderers.AddRange(hits.Select(hit => hit.transform.GetComponent<Renderer>()).Where(renderer => renderer != null).ToList());

		// ** 기존 리스트에는 포함되었지만 현재 ray에 감지된 리스트에는 없는 Renderer
		List<Renderer> extractionList = objectRenderers.Where(renderer => !renderers.Contains(renderer)).ToList();

		// ** 추출이 완료된 Renderer를 기존 알파값으로 되돌린다. 
		// ** 그리고 삭제.
		foreach (Renderer renderer in extractionList)
			SetFadeIn(renderer);

		foreach (RaycastHit hit in hits)
		// ** ray의 충돌이 감지된 Object의 Renderer를 받아옴.
		{
			Renderer renderer = hit.transform.GetComponent<Renderer>();
			// ** 충돌이 있다면 Renderer를 확인.
			if (renderer != null && hit.transform.position != transform.parent.position)
				SetFadeOut(renderer);
		}
	}

	private void SetFadeIn(Renderer renderer)
	{
		objectRenderers.Remove(renderer);

		// ** 변경된 Shader의 Color 값들 받아옴.
		Color color = renderer.material.color;

		color.a = 1.0f;
		renderer.material.color = color;
	}

	private void SetFadeOut(Renderer renderer)
	{
		// ** Color값 변경이 가능한 Shader로 변경.
		objectRenderers.Add(renderer); // ** 추가

		renderer.material = new Material(Shader.Find(path));

		// ** 변경된 Shader의 Color 값들 받아옴.
		Color color = renderer.material.color;

		color.a = 0.0f;
		renderer.material.color = color;
	}
}
