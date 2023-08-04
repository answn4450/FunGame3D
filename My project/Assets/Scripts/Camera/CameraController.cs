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

		Vector3 direction = GameObject.Find("Player").transform.position - transform.position;

		// ** 모든 충돌을 감지.
		hits = Physics.RaycastAll(transform.position, direction, direction.magnitude).ToList();

		// ** 충돌된 모든 원소들 중에 Renderer만 추출한 새로운 리스트를 생성.
		renderers.AddRange(hits.Select(hit => hit.transform.GetComponent<Renderer>()).Where(renderer => renderer != null).ToList());

		// ** 기존 리스트에는 포함되었지만 현재 ray에 감지된 리스트에는 없는 Renderer
		List<Renderer> extractionList = objectRenderers.Where(renderer => !renderers.Contains(renderer)).ToList();

		// ** 추출이 완료된 Renderer를 기존 알파값으로 되돌린다. 
		// ** 그리고 삭제.
		foreach (Renderer renderer in extractionList)
		{
            if (renderer != null)
            {
                objectRenderers.Remove(renderer);
                ApplyOriginalShader(renderer);
            }
		}

		foreach (RaycastHit hit in hits)
		// ** ray의 충돌이 감지된 Object의 Renderer를 받아옴.
		{
			Renderer renderer = hit.transform.GetComponent<Renderer>();
			// ** 충돌이 있다면 Renderer를 확인.
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

		// ** Shader 원 상태로 복구.
		renderer.material = new Material(Shader.Find(defaultShader));

		renderer.material.color = color;
	}

	private void ApplyBlockShader(Renderer renderer)
	{
		Color color = renderer.material.color;

		// ** Color값 변경이 가능한 Shader로 변경.
		renderer.material = new Material(Shader.Find(blockShader));

		color.a = 0.2f;
		renderer.material.color = color;
	}
}