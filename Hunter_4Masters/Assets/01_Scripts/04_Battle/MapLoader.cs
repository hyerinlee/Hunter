using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapLoader : MonoBehaviour
{
    private Grid grid;

    public Tile ground;
    public GameObject spawner;

    public Tilemap tileMap;

    Dictionary<string, string[]> mapJson = new Dictionary<string, string[]>();

    // // 스포너 부분
    // Dictionary<string, MonsterSpawner> spawnerJson = new Dictionary<string, MonsterSpawner>();
    // public MonsterSpawner spawnerData;

    private int num = 0;
    private string[] stages = {"01_stage", "02_stage", "03_stage"};

    public void LoadMap()
    {
        // 맵 데이터
        mapJson = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(Resources.Load<TextAsset>($"Data/mapData").text);
        string[] stage = mapJson[stages[num]];

        // // 스포너 데이터
        // spawnerJson = JsonConvert.DeserializeObject<Dictionary<string, MonsterSpawner>>(Resources.Load<TextAsset>($"Data/monsterSpawner").text);
        // spawnerData = spawnerJson["00_Spawner"];

        string[,] data = new string[stage.Length, stage[0].Length];

        string[] temp= stage[0].Split(',');
        for(int i=0; i<stage.Length; i++)
        {
            temp = stage[i].Split(',');

            for(int j=0; j<temp.Length; j++)
            {
                data[i, j] = temp[j];
            }
        }

        for(int i=0; i<stage.Length; i++)
        {
            for(int j=0; j<temp.Length; j++)
            {
                if(data[i, j].Equals("1"))
                {
                    tileMap.SetTile(new Vector3Int(j, 0-i, 0), ground);
                }
                else if(data[i, j].Equals("0"))
                {
                    tileMap.SetTile(new Vector3Int(j, 0-i, 0), null);
                }
                else if(data[i, j].Equals("S"))
                {
                    tileMap.SetTile(new Vector3Int(j, 0-i, 0), null);

                    MonsterSpawner spawnerScript = spawner.GetComponent<MonsterSpawner>();
                    
                    // spawnerScript.name = spawnerData.name;
                    // spawnerScript.release_Arrange = spawnerData.release_Arrange;
                    // spawnerScript.monsters = spawnerData.monsters;
                    // spawnerScript.spawn_Time = spawnerData.spawn_Time;
                    // spawnerScript.isEmergerd = spawnerData.isEmergerd;
                    // spawnerScript.hp = spawnerData.hp;

                    Instantiate(spawner, new Vector3Int(j+1, 0-i, 0), Quaternion.identity);
                }
                else if(data[i, j].Equals("P"))
                {
                    tileMap.SetTile(new Vector3Int(j, 0-i, 0), null);
                    
                    GameObject player = GameObject.Find("Player");
                    player.transform.position = new Vector3(j+1, 0-i, 0);
                }
            }
        }
        num += 1;
        if(num >= stages.Length) num = 0;
    }
}