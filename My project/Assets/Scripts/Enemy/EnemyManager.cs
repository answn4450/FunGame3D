using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyController> enemyList = new List<EnemyController>();
    private GameObject followerPrefab;
    private int maxChildCount = 16;
    private float breedingTimer;

    private void Awake()
    {
        breedingTimer = 0.0f;
    }

    void Start()
    {
        followerPrefab = PrefabManager.GetInstance().GetPrefabByName("Enemy");

        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; ++i)
                enemyList.Add(transform.GetChild(i).transform.GetComponent<EnemyController>());
        }
        else
        {
            EnemyController enemy = Instantiate(followerPrefab, transform).GetComponent<EnemyController>();
            enemyList.Add(enemy);
        }
    }

    public void TestOnGround()
    {
        int i = 0;
        while (i < 1)
        {
            LivingBall enemy = transform.GetChild(i).GetComponent<LivingBall>();

            enemy.OnGround();
            i++;
        }
    }

    public void LifeCycle()
    {
        DeleteDeadEnemy();

        foreach (EnemyController enemy in enemyList)
        {
            enemy.OnGround();
            enemy.Living();
            enemy.RollPaint();
            breedingTimer += Time.deltaTime;
        }

        if (breedingTimer > 5.0f)
        {
            breedingTimer = 0.0f;
            DoubleBreeding();
        }
    }

    public void FollowPlayer(PlayerController player)
    {
        foreach (EnemyController enemy in enemyList)
        {
            enemy.FollowTarget(player.transform.position);
        }
    }

    private void DeleteDeadEnemy()
    {
        int i = 0;
        while (i < enemyList.Count)
        {
            EnemyController enemy = enemyList[i];
            if (enemy.dead)
            {
                enemyList.RemoveAt(i);
                enemy.Explode();
            }
            else
                ++i;
        }
    }

    private void DoubleBreeding()
    {
        int beforeCount = enemyList.Count;
        
        foreach (EnemyController enemy in enemyList.GetRange(0, beforeCount))
        {
            if (enemyList.Count < maxChildCount)
                enemyList.Add(enemy.Breeding());

        }
    }
}
