using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public float spawnTime = 3f;
    public float curTime;
    public Transform[] spawnPoints;
    public GameObject enemy;

    //private string[,] monsters;

    void Update()
    {
        if(curTime >= spawnTime)
        {
            int x = Random.Range(0, spawnPoints.Length);
            SpawnEnemy(x);
        }
        curTime += Time.deltaTime;
    }

    void SpawnEnemy(int ranNum)
    {
        curTime = 0;
        Instantiate(enemy, spawnPoints[ranNum]);
    }
}