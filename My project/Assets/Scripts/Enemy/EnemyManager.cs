using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyController> enemyList = new List<EnemyController>();
    private GameObject followerPrefab;
    private int maxChildCount = 16;

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

        StartCoroutine(AutoDoubleBreeding());
    }

    public void LifeCycle()
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
            {
                enemy.OnGround();
                enemy.Living();
                enemy.AffectNearGround();
                ++i;
            }
        }    
    }

    public void FollowPlayer(PlayerController player)
    {
        for (int i = 0; i < enemyList.Count; ++i)
        {
            EnemyController enemy = enemyList[i];
            enemyList[i].FollowTarget(player.transform.position);
        }
    }

    private void DoubleBreeding()
    {
        int beforeEnemyCount = enemyList.Count;
        for (int i = 0; i < Mathf.Min(beforeEnemyCount, maxChildCount - beforeEnemyCount); ++i)
        {
            enemyList.Add(enemyList[i].Breeding());
        }
    }

    IEnumerator AutoDoubleBreeding()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            DoubleBreeding();
        }
    }
}
