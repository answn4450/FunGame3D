using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CameraBlockView : MonoBehaviour
{
    private Vector3 direction;
    private Transform target;
    public LayerMask mask;
    public float distance;

    //private const string path = "Custom/Transparent/MyShader";
    private const string path = "Legacy Shaders/Transparent/Specular";

    public List<Renderer> objectRenderers = new List<Renderer>();

    private Vector3[] outCorners = new Vector3[4];

    [Header("Invisibility")]
    public bool FrustumList;

    [Range(0.0f, 1.0f)]
    public float X = 0.45f;

    [Range(0.0f, 1.0f)]
    public float Y = 0.45f;

    [Range(0.0f, 1.0f)]
    public float W = 0.45f;

    [Range(0.0f, 1.0f)]
    public float H = 0.45f;

    private void Awake()
    {
        target = GameObject.Find("Player").transform;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (!target)
        {
            EditorApplication.ExitPlaymode();
            Debug.Log("Target could not be loaded.");
        }

        if (path.Length <= 0)
        {
            EditorApplication.ExitPlaymode();
        }
#endif

        X = 0.48f;
        Y = 0.35f;
        W = 0.04f;
        H = 0.3f;

        FrustumList = true;
        direction = (target.position - transform.position).normalized;
        distance = Vector3.Distance(target.position, transform.position);

        Camera.main.CalculateFrustumCorners(
            new Rect(0, 0, 1, 1),
            Camera.main.farClipPlane,
            Camera.main.stereoActiveEye,
            outCorners
            );
    }

    private void Update()
    {
        if (FrustumList)
        {
            Camera.main.CalculateFrustumCorners(
                new Rect(X, Y, W, H),
                distance,
                Camera.main.stereoActiveEye,
                outCorners
                );

            Debug.DrawLine(transform.position, transform.position + direction * distance, Color.green);

            for (int i = 0; i < 4; ++i)
                Debug.DrawLine(transform.position, (outCorners[i] - transform.position).normalized * distance, Color.blue);
        }

        List<RaycastHit>[] hits = new List<RaycastHit>[4];
        List<Renderer> renderers = new List<Renderer>();

        // ** 모든 충돌을 감지.
        for (int i = 0; i < 4; ++i)
        {
            hits[i] = Physics.RaycastAll(transform.position, direction, distance, mask).ToList();

            // ** 충돌된 모든 원소들 중에 Renderer만 추출한 새로운 리스트를 생성.
            renderers.AddRange(hits[i].Select(hit => hit.transform.GetComponent<Renderer>()).Where(renderer => renderer != null).ToList());
        }

        // ** 기존 리스트에는 포함되었지만 현재 ray에 감지된 리스트에는 없는 Renderer
        List<Renderer> extractionList = objectRenderers.Where(renderer => !renderers.Contains(renderer)).ToList();

        // ** 추출이 완료된 Renderer를 기존 알파값으로 되돌린다. 
        // ** 그리고 삭제.
        foreach (Renderer renderer in extractionList)
        {
            StartCoroutine(SetFadeIn(renderer));
            objectRenderers.Remove(renderer);
        }

        for (int i = 0; i < 4; ++i)
        {
            // ** hits 배열의 모든 원소를 확인.
            foreach (RaycastHit hit in hits[i])
            {
                // ** ray의 충돌이 감지된 Object의 Renderer를 받아옴.
                Renderer renderer = hit.transform.GetComponent<Renderer>();

                // ** 충돌이 있다면 Renderer를 확인.
                if (renderer != null)
                {
                    // ** List에 이미 포함된 Renderer인지 확인.
                    if (!objectRenderers.Contains(renderer))
                    {
                        StartCoroutine(SetFadeOut(renderer));
                    }
                }
            }
        }
    }


    IEnumerator SetFadeOut(Renderer renderer)
    {
        // ** Color값 변경이 가능한 Shader로 변경.
        objectRenderers.Add(renderer); // ** 추가

        renderer.material = new Material(Shader.Find(path));

        // ** 변경된 Shader의 Color값들 받아옴.
        Color color = renderer.material.color;

        // ** color.a 의 값이 0.5f 보다 큰 경우에만 반복.
        while (0.5f < color.a)
        {
            yield return null;

            // ** Alpha(1) -= Time.deltatime;
            color.a -= Mathf.Clamp(Time.deltaTime * 10, 0.1f, 0.5f);

            renderer.material.color = color;
        }
    }


    IEnumerator SetFadeIn(Renderer renderer)
    {
        objectRenderers.Remove(renderer);

        // ** 변경된 Shader의 Color 값들 받아옴.
        Color color = renderer.material.color;

        // ** color.a 값이 0.9f 보다 작은 경우에만 반복
        while (color.a < 0.9f)
        {
            yield return null;
            // ** Alpha(1) -= Time.deltaTime;
            color.a += Mathf.Clamp(Time.deltaTime * 10, 0.1f, 0.5f);

            renderer.material.color = color;
        }

        color.a = 1.0f;
        renderer.material.color = color;
        objectRenderers.Remove(renderer);
    }
}