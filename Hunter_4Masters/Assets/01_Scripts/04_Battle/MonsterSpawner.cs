using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// 스폰 범위
// 스포너 : 2차원 배열 [스테이지][몬스터 종류]

public class Spawner
{
    public int data_ID;
    public string name;
    public float[] release_Arrange;
    public List<List<int>> monsters;
	//public int[,] monsters;
    public float spawn_Time;
	public bool isEmerged;
	public float hp;
    public int spawn_Limit;
}

public class MonsterSpawner : MonoBehaviour
{
    public Dictionary<string, Monster> monsterDict;
    public Dictionary<string, Spawner> spawnerDict;

    public Monster monsterData;
    public Spawner spawnerData;
    
    // public int data_ID;
    // public string name;
    // public float[] release_Arrange;
    // public List<List<int>> monsters;
	// //public int[,] monsters;
    // public float spawn_Time;
	// public bool isEmerged;
	// public float hp;
    // public int spawn_Limit;

    // 몬스터 스폰 여부, 스폰할 인덱스, 몬스터 리스트의 길이
    private bool doSpawn;
    private int monstersIdx;

    // 일단은 여기에 선언해놨는데 스포너가 여럿 설치되면 각기 다른 값을 가질테니까 매니저 스크립트로 이전해야 할듯함 (GameManager 아니면 BattleManager?)
    private int spawnedMonsters = 0;

    private float spawnPoint;
    
    void Awake()
    {
        LoadSpawnerData();
        LoadMonsterData();

        SetSpawnInfo();

        SetSpawner(999998);
        SetSpawnMonsters();
        
        //StartCoroutine(SpawnCoroutine());
    }

    public void MakeSpawner(int data_ID)
    {

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

    // monsterSpawner.json 불러오기
    void LoadSpawnerData()
    {
        spawnerDict = JsonConvert.DeserializeObject<Dictionary<string, Spawner>>(Resources.Load<TextAsset>($"Data/monsterSpawner").text);
        
        // 잘 작동되나 로그 찍어보는 용도
        // foreach(KeyValuePair<string, Spawner> pair in spawnerDict)
        // {
        //     Debug.Log("pair : " + pair + " / pair.key : " + pair.Key + " / pair.value : " + pair.Value);
        //     for (int i = 0; i < spawnerDict[pair.Key].monsters.Count; i++)
        //     {
        //         for(int j=0; j<spawnerDict[pair.Key].monsters[i].Count; j++)
        //         {
        //             //Debug.Log(i + "번째 리스트의 " + j + "번째 요소 : " + spawnerDict[pair.Key].monsters[i][j]);
        //         }
        //     }
        // }
    }

    // 스폰 여부를 결정할 키값들 초기화
    void SetSpawnInfo()
    {
        doSpawn = true;
        monstersIdx = 0;
    }

    // 설치할 스포너 세팅
    void SetSpawner(int spawnerDataID)
    {
        foreach(KeyValuePair<string, Spawner> pair in spawnerDict)
        {
            if(spawnerDict[pair.Key].data_ID == spawnerDataID)
            {
                // 설치할 스포너가 정해짐
                spawnerData = spawnerDict[pair.Key];
                Debug.Log("설치할 스포너 data_ID : " + spawnerData.data_ID);
                Debug.Log("설치할 스포너 name : " + spawnerData.name);
                Debug.Log("설치할 스포너 spawn_limit : " + spawnerData.spawn_Limit);
            }
        }
    }

    // 스포너의 Monsters에 해당하는 몬스터들 체크
    void SetSpawnMonsters()
    {
        Debug.Log("SetSpawnMonsters실행중");
        // for (int i = 0; i<spawnerData.monsters.Count; i++)
        // {
        //     for(int j=0; j<spawnerData.monsters[i].Count; j++)
        //     {
        //         Debug.Log(spawnerData.name + "의 monsters 중 " + i + "번째 리스트의 " + j + "번째 요소 : " + spawnerData.monsters[i][j]);
        //         foreach(KeyValuePair<string, Monster> pair in monsterDict)
        //         {
        //             if(monsterDict[pair.Key].data_ID == spawnerData.monsters[i][j])
        //             {
        //                 SpawnMonster(pair.Key);
        //                 Debug.Log(pair.Key + " 소환!");
        //             }
                        
        //         }
        //     }
        // }


        // Debug.Log("spawnerData.monsters[monstersIdx].Count 의 길이 : " + spawnerData.monsters[monstersIdx].Count);
        // Debug.Log("spawnedMonsters 의 크기 : " + spawnedMonsters);

        // 이번에 스폰할 리스트의 길이와 현재 소환되어져 있는 몬스터 수의 합이 spawn Limit 을 넘지 않다면
        if((spawnerData.monsters[monstersIdx].Count + spawnedMonsters) <= spawnerData.spawn_Limit)
        {
            doSpawn = true;
            Debug.Log("몬스터를 소환할 수 있습니다.");
        }
        else
        {
            doSpawn = false;
            Debug.Log("몬스터 소환ㄴㄴ");
            return; // 지금은 리턴 해주는데 실제 플레이에서는 지워야 할 듯 전투 끝나기 전까지 스폰은 계속 해야하니깐 .. ?
        }

        if(doSpawn)
        {
            for(int j=0; j<spawnerData.monsters[monstersIdx].Count; j++)
            {
                //Debug.Log(spawnerData.name + "의 monsters 중 " + monstersIdx + "번째 리스트의 " + j + "번째 요소 : " + spawnerData.monsters[monstersIdx][j]);
                foreach(KeyValuePair<string, Monster> pair in monsterDict)
                {
                    if(monsterDict[pair.Key].data_ID == spawnerData.monsters[monstersIdx][j])
                    {
                        SpawnMonster(pair.Key);
                        //Debug.Log(pair.Key + " 소환!");
                    }    
                }
            }

            // monsterssIdx 값을 랜덤으로 설정
            monstersIdx = UnityEngine.Random.Range(0, spawnerData.monsters.Count);
            // monstersIdx++;
            // if(monstersIdx >= spawnerData.monsters.Count) monstersIdx = 0;
            Debug.Log("monstersIdx : " + monstersIdx);
        }

        // 시간 지연이 필요 없으므로 곧바로 호출
        SetSpawnMonsters();
    }

    // monsterData.json 불러오기
    void LoadMonsterData()
    {
        // 2차원 배열 생각 안하고 만들음... 따라서 한 번 더 감싸야 하는...
        monsterDict = JsonConvert.DeserializeObject<Dictionary<string, Monster>>(Resources.Load<TextAsset>($"Data/monsterData").text);
        
        // Debug.Log("LoadMonsterData 함수 실행 후 monsterDict 값 : " + monsterDict + "monsterDict의 요소들 : ");
        // foreach(KeyValuePair<string, Monster> pair in monsterDict)
        // {
        //     Debug.Log("pair : " + pair + " pair.key : " + pair.Key);
        // }
    }

    void SpawnMonster(string monsterName)
    {
        try {
            // 데이터 및 프리팹 세팅
            monsterData = monsterDict[monsterName];
            GameObject monster = Resources.Load(monsterName) as GameObject;

            // 스크립트 불러오기
            string scriptName = monsterName.Substring(3);
            //Debug.Log(scriptName);
            Monster script = monster.GetComponent(scriptName) as Monster;

            // 몬스터 데이터 설정
            SetMonsterData(script, monsterData);

            // 몬스터 생성
            // 스폰 범위
            //spawnPoint = Random.Range(release_Arrange[0], release_Arrange[1]);
            Debug.Log("스폰위치");
            Debug.Log(spawnPoint);
            Vector2 spawnVector = new Vector2(transform.position.x + spawnPoint, transform.position.y);
            Instantiate(monster, spawnVector, Quaternion.identity);
        } catch(NullReferenceException e) {
            Debug.Log("스폰 오류. 조건에 맞는 프리팹이 없음. " +  monsterName + "<- data_Id : " + monsterDict[monsterName].data_ID);
        }

        spawnedMonsters++;
    }

    void SetMonsterData(Monster target, Monster data)
    {
        target.data_ID = data.data_ID;
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