using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public class PlayerData
{
    public Dictionary<string, float> foster_data;
    public List<ItemData> items;

    public PlayerData DeepCopy()
    {
        PlayerData other = (PlayerData)this.MemberwiseClone();
        other.foster_data = new Dictionary<string, float>(foster_data);
        other.items = new List<ItemData>(items);
        return other;
    }
}

public class FosterManager : Singleton<FosterManager>
{

    private PlayerData playerData;

    private void Start()
    {
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        playerData = JsonConvert.DeserializeObject<PlayerData>(Resources.Load<TextAsset>($"Data/PlayerData").text);

    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerData pd)
    {
        playerData = pd;
    }
}
