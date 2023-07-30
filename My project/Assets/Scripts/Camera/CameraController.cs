using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
	[SerializeField, Min(0f)]
	float
		springStrength = 100f,
		dampingStrength = 10f,
		//jostleStrength = 40f,
		pushStrength = 1f;

	private Vector3 anchorPosition, velocity;

	private const float basicFieldViewDegree = 50.0f;
	private float baseY0;
	private float Length;
	private PlayerController Player;
	private float swivelDegZ;
	private float fieldViewDegree;

	private List<Renderer> objectRenderers = new List<Renderer>();
	private const string blockShader = "Legacy Shaders/Transparent/Specular";
	private const string defaultShader = "Standard";
	
	private void Awake()
	{
		baseY0 = 1.0f;
		Length = 8.0f;
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
		CleanBlockView();
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

	private void BindTransform()
	{
		swivelDegZ = Mathf.Lerp(swivelDegZ, 0.0f, Time.deltaTime);
		fieldViewDegree = Mathf.Lerp(fieldViewDegree, basicFieldViewDegree, Time.deltaTime);
	}

	private void CleanBlockView()
	{
		List<RaycastHit> hits = new List<RaycastHit>();
		List<Renderer> renderers = new List<Renderer>();

		Vector3 direction = transform.parent.position - transform.position;

		// ** ��� �浹�� ����.
		hits = Physics.RaycastAll(transform.position, direction, direction.magnitude).ToList();

		// ** �浹�� ��� ���ҵ� �߿� Renderer�� ������ ���ο� ����Ʈ�� ����.
		renderers.AddRange(hits.Select(hit => hit.transform.GetComponent<Renderer>()).Where(renderer => renderer != null).ToList());

		// ** ���� ����Ʈ���� ���ԵǾ����� ���� ray�� ������ ����Ʈ���� ���� Renderer
		List<Renderer> extractionList = objectRenderers.Where(renderer => !renderers.Contains(renderer)).ToList();

		// ** ������ �Ϸ�� Renderer�� ���� ���İ����� �ǵ�����. 
		// ** �׸��� ����.
		foreach (Renderer renderer in extractionList)
		{
			objectRenderers.Remove(renderer);
			ApplyOriginalShader(renderer);
		}

		foreach (RaycastHit hit in hits)
		// ** ray�� �浹�� ������ Object�� Renderer�� �޾ƿ�.
		{
			Renderer renderer = hit.transform.GetComponent<Renderer>();
			// ** �浹�� �ִٸ� Renderer�� Ȯ��.
			if (renderer != null && hit.transform.tag != "Player")
			{
				if (!objectRenderers.Contains(renderer))
					objectRenderers.Add(renderer);

				ApplyBlockShader(renderer);
			}
		}
	}

	private void ApplyOriginalShader(Renderer renderer)
	{
		Color color = renderer.material.color;
		color.a = 1.0f;

		// ** Shader �� ���·� ����.
		renderer.material = new Material(Shader.Find(defaultShader));

		renderer.material.color = color;
	}

	private void ApplyBlockShader(Renderer renderer)
	{
		Color color = renderer.material.color;

		// ** Color�� ������ ������ Shader�� ����.
		renderer.material = new Material(Shader.Find(blockShader));

		color.a = 0.2f;
		renderer.material.color = color;
	}
}