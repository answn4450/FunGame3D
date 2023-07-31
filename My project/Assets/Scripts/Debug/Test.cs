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
		// ** ��� �浹�� ����.
		hits = Physics.RaycastAll(transform.position, direction, direction.magnitude).ToList();
		
		// ** �浹�� ��� ���ҵ� �߿� Renderer�� ������ ���ο� ����Ʈ�� ����.
		renderers.AddRange(hits.Select(hit => hit.transform.GetComponent<Renderer>()).Where(renderer => renderer != null).ToList());

		// ** ���� ����Ʈ���� ���ԵǾ����� ���� ray�� ������ ����Ʈ���� ���� Renderer
		List<Renderer> extractionList = objectRenderers.Where(renderer => !renderers.Contains(renderer)).ToList();

		// ** ������ �Ϸ�� Renderer�� ���� ���İ����� �ǵ�����. 
		// ** �׸��� ����.
		foreach (Renderer renderer in extractionList)
			SetFadeIn(renderer);

		foreach (RaycastHit hit in hits)
		// ** ray�� �浹�� ������ Object�� Renderer�� �޾ƿ�.
		{
			Renderer renderer = hit.transform.GetComponent<Renderer>();
			// ** �浹�� �ִٸ� Renderer�� Ȯ��.
			if (renderer != null && hit.transform.position != transform.parent.position)
				SetFadeOut(renderer);
		}
	}

	private void SetFadeIn(Renderer renderer)
	{
		objectRenderers.Remove(renderer);

		// ** ����� Shader�� Color ���� �޾ƿ�.
		Color color = renderer.material.color;

		color.a = 1.0f;
		renderer.material.color = color;
	}

	private void SetFadeOut(Renderer renderer)
	{
		// ** Color�� ������ ������ Shader�� ����.
		objectRenderers.Add(renderer); // ** �߰�

		renderer.material = new Material(Shader.Find(path));

		// ** ����� Shader�� Color ���� �޾ƿ�.
		Color color = renderer.material.color;

		color.a = 0.0f;
		renderer.material.color = color;
	}
}
