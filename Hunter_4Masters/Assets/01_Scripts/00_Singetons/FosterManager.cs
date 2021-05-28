using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq;
using Newtonsoft.Json;

#region Define Foster Classes
public class PlayerData : ICloneable
{
    public Dictionary<string, Stat> Stats;
    public Dictionary<string, Stat> Cons;
    public MonInven Mon_Inven;

    public object Clone()
    {
        PlayerData playerData = new PlayerData();
        playerData.Stats = new Dictionary<string, Stat>();
        playerData.Cons = new Dictionary<string, Stat>();
        foreach (KeyValuePair<string, Stat> pair in Stats)
        {
            playerData.Stats.Add(pair.Key, pair.Value.Clone() as Stat);
        }
        foreach (KeyValuePair<string, Stat> pair in Cons)
        {
            playerData.Cons.Add(pair.Key, pair.Value.Clone() as Stat);
        }
        playerData.Mon_Inven = this.Mon_Inven.Clone() as MonInven;
        return playerData;
    }

    public float GetCurPointOfAllType(string key)
    {
        if (Stats.ContainsKey(key))
        {
            return Stats[key].cur_point;
        }
        else if (Cons.ContainsKey(key))
        {
            return Cons[key].cur_point;
        }
        else if (key == "money") return Mon_Inven.Money;
        else Debug.LogError(key + "에 해당하는 플레이어 데이터가 존재하지 않음");
        return 0;
    }

    public void AddToCurPointOfAllType(string key, float value)
    {
        if (Stats.ContainsKey(key))
        {
            Stats[key].cur_point += value;
        }
        else if (Cons.ContainsKey(key))
        {
            Cons[key].cur_point += value;
        }
        else if (key == "money") Mon_Inven.Money += value;
        else Debug.LogError(key + "에 해당하는 플레이어 데이터가 존재하지 않음");
    }

    public string GetStat(string key)
    {
        int effectValue=0;
        for(int i=0; i < Mon_Inven.Equipment.Count; i++)
        {
            if (Mon_Inven.Equipment[i].item_name == "none") continue;
            ItemData itemData = DataManager.Instance.GetItemData(Mon_Inven.Equipment[i].item_name);
            for (int j=0; j<itemData.effect.Count; j++)
            {
                if (itemData.effect[j].effect_variable == key) effectValue += (int)itemData.effect[j].effect_min;
            }
        }
        return Stats[key].name + ": " + Stats[key].cur_point +
            "[" + (Stats[key].cur_point-effectValue) + "(/" + Stats[key].max_point + ")" +
            string.Format("{0:+0;-0}", effectValue) + "]";
    }

    public string GetBattleStat()
    {
        string atk = Stats["ATK"].name + " " + Stats["ATK"].cur_point;
        string aps = Stats["APS"].name + " " + Stats["APS"].cur_point.ToString("F2");
        string def = Stats["DEF"].name + " " + Stats["DEF"].cur_point;
        return string.Format("{0, -5} {1, -5} {2, -5}", atk, aps, def);
    }

    public string GetStateOutOfMax(string key)
    {
        return Cons[key].cur_point + "/" + Cons[key].max_point;
    }

    public string GetMoney()
    {
        return string.Format("{0:#,##0}", Mon_Inven.Money) + "$";
    }

    public void AddInvenItem(string item, int itemEach)   // argument = ItemDataDict's key
    {
        Mon_Inven.Inventory = Mon_Inven.Inventory.OrderBy(x=>x.inven_index).ToList();
        int firstEmptyIndex = 0;
        int index = -1;
        for(int i=0; i < Mon_Inven.Inventory.Count; i++)
        {
            if(DataManager.Instance.GetItemData(item).type=="consumable" &&
                Mon_Inven.Inventory[i].item_name == item) index = i; // (소비아이템 한정) 이미 있는 아이템인지 체크
            if (firstEmptyIndex == Mon_Inven.Inventory[i].inven_index) firstEmptyIndex++;

        }

        if (index != -1)
        {
            Mon_Inven.Inventory[index].item_each += itemEach;
        }
        else Mon_Inven.Inventory.Add(new InvenItem()
        {
            inven_index = firstEmptyIndex,
            item_name = item,
            item_each = itemEach
        });
    }

    public void AddInvenItemAt(int index, string item, int itemEach)
    {
        Mon_Inven.Inventory.Add(new InvenItem()
        {
            inven_index = index,
            item_name = item,
            item_each = itemEach
        });
    }

    public void RemoveInvenItem(PlayerItem item, int itemEach)
    {
        for(int i=0; i < Mon_Inven.Inventory.Count; i++)
        {
            if(((InvenItem)item).inven_index == Mon_Inven.Inventory[i].inven_index)
            {
                if (Mon_Inven.Inventory[i].item_each == itemEach) Mon_Inven.Inventory.RemoveAt(i);
                else Mon_Inven.Inventory[i].item_each -= itemEach;
            }
        }
    }

    public void SetEquipItem(int index, string item)
    {
        if (Mon_Inven.Equipment[index].item_name != "none") ResetEquipItem(index);
        ItemData itemData = DataManager.Instance.GetItemData(item);
        for(int i=0; i<itemData.effect.Count; i++)
        {
            AddToCurPointOfAllType(itemData.effect[i].effect_variable, itemData.effect[i].effect_min);
        }
        Mon_Inven.Equipment[index].item_name = item;
        Mon_Inven.Equipment[index].item_each = 1;
    }

    public void ResetEquipItem(int index)
    {
        ItemData itemData = DataManager.Instance.GetItemData(Mon_Inven.Equipment[index].item_name);
        for (int i = 0; i < itemData.effect.Count; i++)
        {
            AddToCurPointOfAllType(itemData.effect[i].effect_variable, -itemData.effect[i].effect_min);
        }
        Mon_Inven.Equipment[index].item_name = "none";
        Mon_Inven.Equipment[index].item_each = 0;
    }
}

public class Stat : ICloneable
{
    public string name;
    public float max_point;
    public float cur_point;

    public object Clone()
    {
        return new Stat
        {
            name = this.name,
            max_point = this.max_point,
            cur_point = this.cur_point
        };
    }
}

public class MonInven : ICloneable
{
    public float Money;
    public List<EquipItem> Equipment;
    public List<InvenItem> Inventory;

    public object Clone()
    {
        MonInven monInven = new MonInven();
        monInven.Money = this.Money;
        monInven.Equipment = new List<EquipItem>();
        monInven.Inventory = new List<InvenItem>();

        for(int i=0; i < Equipment.Count; i++)
        {
            monInven.Equipment.Add(Equipment[i].Clone() as EquipItem);
        }
        for (int i = 0; i < Inventory.Count; i++)
        {
            monInven.Inventory.Add(Inventory[i].Clone() as InvenItem);
        }
        return monInven;
    }
}

public class EquipItem : PlayerItem, ICloneable
{
    public int equip_index;

    public object Clone()
    {
        return new EquipItem
        {
            equip_index = this.equip_index,
            item_name = this.item_name,
            item_each = this.item_each
        };
    }
}

public class InvenItem : PlayerItem, ICloneable
{
    public int inven_index;

    public object Clone()
    {
        return new InvenItem
        {
            inven_index = this.inven_index,
            item_name = this.item_name,
            item_each = this.item_each
        };
    }
}

public class PlayerItem
{
    public string item_name;
    public int item_each;
}
#endregion

public class FosterManager : Singleton<FosterManager>
{

    private PlayerData playerData;

    public override void Awake()
    {
        base.Awake();
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        Dictionary<string, PlayerData> data = JsonConvert.DeserializeObject<Dictionary<string, PlayerData>>(Resources.Load<TextAsset>($"Data/player_base").text);
        playerData = data.Values.ElementAt(0);   // KeyValuePair<> 형태로 역직렬화 불가-> Dictionary<>로 받음
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
