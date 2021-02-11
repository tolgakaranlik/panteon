using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] EnemyModels;
    public int MaxEnemies;
    public float SpawnDelay = 0.5f;

    List<Transform> SpawnedEnemies;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedEnemies = new List<Transform>();
        SpawnFirstEnemy();
    }

    private void SpawnFirstEnemy()
    {
        StartCoroutine(SpawnNextEnemy(0));
    }

    public int GetOrder(float playerZ)
    {
        int onesAhead = 0;

        for (int i = 0; i < SpawnedEnemies.Count; i++)
        {
            if(SpawnedEnemies[i].position.z > playerZ)
            {
                onesAhead += 1;
            }
        }

        return onesAhead;
    }

    IEnumerator SpawnNextEnemy(int index)
    {
        if(index < MaxEnemies)
        {
            yield return new WaitForSeconds(SpawnDelay);

            Debug.Log("Spawining as new enemy");
            var enemy = Instantiate(EnemyModels[UnityEngine.Random.Range(0, EnemyModels.Length)], new Vector3(-0.22f + UnityEngine.Random.Range(0, 0.40f), 0.005f, -0.20f), Quaternion.identity, transform);
            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = new Vector3(enemy.transform.position.x, enemy.transform.position.y, 16f);
            enemy.GetComponent<EnemyController>().PathStarted = true;

            SpawnedEnemies.Add(enemy.transform);

            var animator = enemy.GetComponent<Animator>();
            animator.SetBool("isStanding", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isDead", false);

            animator.CrossFade("Run", 0.01f);
            StartCoroutine(SpawnNextEnemy(index + 1));
        }
    }
}
