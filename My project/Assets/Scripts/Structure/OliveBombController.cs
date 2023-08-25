using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OliveBombController : Structure
{
    private List<EnemyController> enemyInRange;
    private List<GroundController> groundInRange;

    private void Awake()
    {
        enemyInRange = new List<EnemyController>();
        groundInRange = new List<GroundController>();
    }

    private void Update()
    {
        float last = 2.3f - transform.localScale.x;
        if (last > 0.4f)
        {
            transform.localScale += Vector3.one * last * Time.deltaTime * 2.0f;

            int i = 0;
            while (i + 1 < enemyInRange.Count)
            {
                EnemyController enemy = enemyInRange[i];
                if (enemy)
                {
                    i++;
                    enemy.Hurt();
                }
                else
                    enemyInRange.RemoveAt(i);
            }

            i = 0;
            while (i + 1 < groundInRange.Count)
            {
                GroundController ground = groundInRange[i];
                if (ground)
                {
                    i++;
                    float distance_y = Mathf.Abs(
                        transform.position.y - Tools.GetInstance().GetTopY(ground.transform)
                        );
                    float distance_x = Mathf.Abs(
                        transform.position.x - ground.transform.position.x
                        );
                    float power = Mathf.Max(0.0f, 10.0f - distance_x * distance_y);
                    ground.AttackGround(power * Time.deltaTime);
                }
                else
                    groundInRange.RemoveAt(i);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        GroundController ground = other.GetComponent<GroundController>();

        if (enemy && !enemyInRange.Contains(enemy))
            enemyInRange.Add(enemy);

        if (ground && !groundInRange.Contains(ground))
            groundInRange.Add(ground);

    }

    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        GroundController ground = other.GetComponent<GroundController>();

        if (enemy && enemyInRange.Contains(enemy))
            enemyInRange.Remove(enemy);

        if (ground && groundInRange.Contains(ground))
            groundInRange.Remove(ground);

    }
}
