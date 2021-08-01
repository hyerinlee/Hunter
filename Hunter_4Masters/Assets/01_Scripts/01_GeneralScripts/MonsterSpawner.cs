using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스폰 범위
// 스포너 : 2차원 배열 [스테이지][몬스터 종류]

public class MonsterSpawner : MonoBehaviour
{
    public float spawnTime = 3f;
    public float curTime;
    public Transform[] spawnPoints;
    public GameObject[] enemy;

    int i=0;

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
        Instantiate(enemy[i], spawnPoints[ranNum]);
        i++;
        if(i>enemy.Length) i=0;
    }
}