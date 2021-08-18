using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

// 스폰 범위
// 스포너 : 2차원 배열 [스테이지][몬스터 종류]

public class MonsterSpawner : MonoBehaviour
{
    public Dictionary<string, Monster> monsterDict;
    public Monster monsterData;
    
    private int count = 0;
    private int max = 3;
    
    void Awake()
    {
        LoadMonsterData();
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        foreach(KeyValuePair<string, Monster> pair in monsterDict)
        {
            SpawnMonster(pair.Key);
            yield return new WaitForSeconds(3.0f);
        }

        StartCoroutine(SpawnCoroutine());
    }

    void LoadMonsterData()
    {
        // 2차원 배열 생각 안하고 만들음... 따라서 한 번 더 감싸야 하는...
        monsterDict = JsonConvert.DeserializeObject<Dictionary<string, Monster>>(Resources.Load<TextAsset>($"Data/monsterData").text);
    }

    void SpawnMonster(string monsterName)
    {
        // 데이터 및 프리팹 세팅
        monsterData = monsterDict[monsterName];
        GameObject monster = Resources.Load(monsterName) as GameObject;

        // 스크립트 불러오기
        string scriptName = monsterName.Substring(0, 1).ToUpper() + monsterName.Substring(1);
        Monster script = monster.GetComponent(scriptName) as Monster;

        // 몬스터 데이터 설정
        SetMonsterData(script, monsterData);

        // 몬스터 생성
        // 스폰 범위 추가 필요
        Instantiate(monster, transform.position, Quaternion.identity);
    }

    void SetMonsterData(Monster target, Monster data)
    {
        target.name = data.name;
        target.hp = data.hp;
        target.atk = data.atk;
        target.aps = data.aps;
        target.def = data.def;
        target.spd = data.spd;
        target.attackRange = data.attackRange; // 수정 필요
        target.moveLimit = data.moveLimit;
    }
}