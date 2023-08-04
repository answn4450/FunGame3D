using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameObject player;
    Vector3 originalPosition;

    public EnemyController parent;
    public EnemyController child;
    public int familyCount;
    public int maxFamilyCount;
    public LayerMask mask;
    private GameObject destroyFX;
    private bool dead;
    private float size;
    private const float deadSize = 0.2f;
    private void Awake()
    {
        parent = null;
        child = null;
        familyCount = 1;
        maxFamilyCount = 12;
        dead = false;
        size = 1.0f;
    }

    void Start()
    {
        mask = Tools.GetInstance().groundMask;
        player = GameObject.Find("Player");
        originalPosition = transform.position;
        StartCoroutine(AutoBreeding());
        destroyFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
    }

    public void Update()
    {
        FollowTarget(player.transform.position);
        CheckDead();
        SphereBySize(size);
        if (dead)
            Explode();
        AffectNearGround();
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
            other.transform.GetComponent<PlayerController>().Hurt();
	}

    IEnumerator AutoBreeding()
    {
        while (true)
        {
            yield return familyCount <= maxFamilyCount ? new WaitForSeconds(3.0f) : null;
            if (!parent)
			{
                if (familyCount < maxFamilyCount)
                    DoubleBreeding(familyCount);
                if (familyCount > maxFamilyCount)
                    Explode();
			}
        }
    }

	private void FollowTarget(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float step = Time.deltaTime * 0.01f;
        transform.position += direction.normalized * direction.magnitude * step;
    }
    
    private void Explode()
    {
        if (child)
            child.parent = null;
        
        Destroy(gameObject);
        --familyCount;
        PassFamilyCount(familyCount);
        Instantiate(destroyFX, transform.position, transform.rotation);
    }

    private void DoubleBreeding(int count)
	{
        if (count > 0)
		{
            --count;
            Breeding();
            child.DoubleBreeding(count);
            Ground.GetInstance().groundMinimumHeight += 0.1f;
		}
	}

    public void Hurt()
    {
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;

    }

    private void Breeding()
    {
        EnemyController lastChild = GetComponent<EnemyController>();
        while (lastChild.child)
            lastChild = lastChild.child;

        familyCount += 1;
        lastChild.child = InstantiateSelf().GetComponent<EnemyController>();
        lastChild.child.parent = lastChild;
        PassFamilyCount(familyCount);
    }

    private GameObject InstantiateSelf()
    {
        return Instantiate(
            gameObject, originalPosition, Quaternion.identity, 
            transform.parent.transform
            );
    }

    private void PassFamilyCount(int _newCount)
    {
        PassFamilyCountToParent(_newCount);
        PassFamilyCountToChild(_newCount);
    }

    private void PassFamilyCountToParent(int _newCount)
    {
        familyCount = _newCount;
        
        if (child)
            child.PassFamilyCountToParent(_newCount);
    }

    private void PassFamilyCountToChild(int _newCount)
    {
        familyCount = _newCount;

        if (parent)
            parent.PassFamilyCountToChild(_newCount);
    }

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private void CheckDead()
    {
        bool before = dead;
        dead = size <= deadSize;
        if (dead && !before)
            Explode();
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }

    private void SphereBySize(float size)
    {
        float newSize = Mathf.Lerp(
            transform.localScale.x,
            size,
            Time.deltaTime
            );

        if (Mathf.Abs(size - newSize) < 0.01)
            newSize = size;

        SetSphere(newSize);
    }

    private void AffectNearGround()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 2.0f, Color.white);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, Tools.GetInstance().groundMask))
            hit.transform.GetComponent<GroundController>().MoreEvilGround();
    }
}
