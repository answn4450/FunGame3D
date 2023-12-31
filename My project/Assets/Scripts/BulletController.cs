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
    private string paintOwnerName;

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
        Tools.GetInstance().AddGroundUpper(transform);
	}

	void Update()
    {
        if (!stop)
            transform.position += speed * Time.deltaTime * transform.forward;

        if (OutOfBound())
            DestroySelf();
    }

    public void BirthBullet(GameObject parent)
    {
        transform.forward = parent.transform.forward;
        transform.position = parent.transform.position;
        parentName = parent.transform.name;
        if (parent.name == "Player")
            paintOwnerName = Status.GetInstance().playerName;
        else
            paintOwnerName = Status.GetInstance().enemyName;
    }

    public void BirthBullet(GameObject parent, bool playerSide)
    {
        transform.forward = parent.transform.forward;
        transform.position = parent.transform.position;
        parentName = parent.transform.name;
        if (playerSide)
            paintOwnerName = Status.GetInstance().playerName;
        else
            paintOwnerName = Status.GetInstance().enemyName;
    }

    public void RideWithPlayer(GameObject player)
    {
        transportPlayer = true;
        player.transform.position = transform.position;
    }

    public void DestroySelf()
	{
        GameObject fx = Instantiate(destroyFX);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Structure")
            DestroySelf();
        else if (released)
        {
            if (other.transform.CompareTag("Player") && !transportPlayer)
            {
                other.GetComponent<PlayerController>().Hurt();
            }
            else if (other.transform.CompareTag("Ground"))
            {
                other.GetComponent<GroundController>().AttackGround(2.0f * Time.deltaTime);
                other.GetComponent<GroundController>().PaintColor(paintOwnerName, 2.0f);
            }
            else if (other.transform.GetComponent<EnemyController>())
                other.transform.GetComponent<EnemyController>().Hurt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!released && other.transform.name == parentName)
            released = true;
        else if (released)
		{
            if (!other.transform.CompareTag("Player"))
                DestroySelf();
            else if (!transportPlayer)
                DestroySelf();
        }
    }

    private bool OutOfBound()
	{
        float radius = transform.localScale.x * 0.5f;
        if (transform.position.x - radius < Ground.GetInstance().groundX0)
            return true;
        if (transform.position.x + radius > Ground.GetInstance().groundX1)
            return true;
        if (transform.position.z - radius < Ground.GetInstance().groundZ0)
            return true;
        if (transform.position.z + radius > Ground.GetInstance().groundZ1)
            return true;

        return false;
    }
}
