using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : LivingBall
{
    Vector3 originalPosition;

    public LayerMask mask;
    public bool dead;
    private GameObject destroyFX;
    private float size;
    private const float deadSize = 0.2f;

    private void Awake()
    {
        dead = false;
        size = 1.0f;
    }

    void Start()
    {
        mask = LayerMask.GetMask("Ground");
        originalPosition = transform.position;
        destroyFX = PrefabManager.GetInstance().GetPrefabByName("CFXR Hit A (Red)");
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
            other.transform.GetComponent<PlayerController>().Hurt();
	}

    public void Living()
    {
        CheckDead();
        BackToSize(size);
    }
   
    public void Explode()
    {
        Destroy(gameObject);
        Instantiate(destroyFX, transform.position, transform.rotation);
    }

	public void FollowTarget(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float step = Time.deltaTime * 0.1f;
        Vector3 move = direction.normalized * direction.magnitude * step;
        SafeMove(move);
    }

    public void RollPaint()
    {
        foreach (GroundController ground in GetTouchingGrounds())
            ground.PaintColor(Status.GetInstance().enemyName, Time.deltaTime);
    }

    public void Hurt()
    {
        if (size > deadSize)
            size -= 0.1f;
        if (size < deadSize)
            size = deadSize;
    }

    public EnemyController Breeding()
    {
        return InstantiateSelf().GetComponent<EnemyController>();
    }

    private GameObject InstantiateSelf()
    {
        return Instantiate(
            gameObject, originalPosition, Quaternion.identity, 
            transform.parent.transform
            );
    }

    private void SetSphere(float r)
    {
        transform.localScale = GetSphere(r);
    }

    private Vector3 GetSphere(float r)
    {
        return new Vector3(r, r, r);
    }

    private void CheckDead()
    {
        dead = size <= deadSize;
    }

    private void BackToSize(float size)
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
}
