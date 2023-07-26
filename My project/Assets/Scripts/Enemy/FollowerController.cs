using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoBehaviour
{
    public Transform folder;
    GameObject player;
    Vector3 originalPosition;

    public FollowerController parent;
    public FollowerController child;
    public int familyCount;
    public int maxFamilyCount;
    private GameObject destroyFX;

    private void Awake()
    {
        parent = null;
        child = null;
        familyCount = 1;
        maxFamilyCount = 2;
    }

    void Start()
    {
        player = GameObject.Find("Player");
        originalPosition = transform.position;
        StartCoroutine(AutoBreeding());
        destroyFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
    }

    void Update()
    {
        FollowTarget(player.transform.position);
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

    IEnumerator AutoBreeding()
    {
        while (true)
        {
            yield return familyCount <= maxFamilyCount ? new WaitForSeconds(3.0f) : null;
            Breeding();
            if (familyCount > maxFamilyCount && !parent)
                Explode();
        }
    }

    private void Breeding()
    {
        FollowerController lastChild = GetComponent<FollowerController>();
        while (lastChild.child)
            lastChild = lastChild.child;

        familyCount += 1;
        lastChild.child = InstantiateSelf().GetComponent<FollowerController>();
        lastChild.child.parent = lastChild;
        PassFamilyCount(familyCount);
    }

    private GameObject InstantiateSelf()
    {
        return Instantiate(
            gameObject, originalPosition, Quaternion.identity, 
            folder);
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

}
