using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [HideInInspector]
    private float speed;
    private bool stop;
    private string parentName;
    private bool released;
    private bool transportPlayer;

    private GameObject destroyFX;

    private void Awake()
    {
        speed = 10.0f;
        stop = false;
        released = false;
        transportPlayer = false;
    }

	private void Start()
	{
        destroyFX = PrefabManager.GetInstance().GetPrefabByName("CFXR3 Hit Fire B (Air)");
	}

	void Update()
    {
        if (!stop)
            transform.position += transform.forward * speed * Time.deltaTime;

        if (OutOfBound())
            DestroySelf();
    }

    public void BirthBullet(GameObject parent)
    {
        transform.forward = parent.transform.forward;
        transform.position = parent.transform.position;
        parentName = parent.transform.name;
    }

    public void RideWithPlayer(GameObject player)
    {
        transportPlayer = true;
        player.transform.position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Struct")
            DestroySelf();
        else if (released)
        {
            StartCoroutine(KnockBack(other.gameObject, 3.0f));
            if (other.tag == "Player" && !transportPlayer)
            {
                other.GetComponent<PlayerController>().Hurt();
            }
            else if (other.tag == "Ground")
                other.GetComponent<GroundController>().SqueezeGround();
            else if (other.transform.GetComponent<FollowerController>())
                other.transform.GetComponent<FollowerController>().Hurt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!released && other.transform.name == parentName)
            released = true;
        else if (released)
		{
            if (other.tag != "Player")
                DestroySelf();
            else if (!transportPlayer)
                DestroySelf();
        }
    }

    private void DestroySelf()
	{
        GameObject fx = Instantiate(destroyFX);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }

    private bool OutOfBound()
	{
        float size = transform.localScale.x;
        if (transform.position.x + size < Status.GetInstance().groundX0)
            return true;
        if (transform.position.x - size > Status.GetInstance().groundX1)
            return true;
        if (transform.position.z + size < Status.GetInstance().groundZ0)
            return true;
        if (transform.position.z - size > Status.GetInstance().groundZ1)
            return true;

        return false;
    }

    IEnumerator KnockBack(GameObject hit, float t)
    {
        while (t>0.0f && hit != null && gameObject != null)
        {
            yield return null;
            if (gameObject != null)
                hit.transform.position += transform.forward * t * Time.deltaTime;
            t -= Time.deltaTime;
        }

        if (gameObject!=null)
            Destroy(gameObject);
    }

    IEnumerator test(float a)
    {
        while (a > 0.0f)
        {
            yield return null;
            a -= Time.deltaTime;
            Debug.Log(a);
        }
    }
}
