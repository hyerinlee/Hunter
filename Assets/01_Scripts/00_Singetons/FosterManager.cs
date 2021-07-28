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
    public Con Cons;
    public MonInven Mon_Inven;

    public object Clone()
    {
        PlayerData clone = new PlayerData();
        clone.Stats = new Dictionary<string, Stat>();
        clone.Cons = (Con)this.Cons.Clone();
        foreach (KeyValuePair<string, Stat> pair in Stats)
        {
            clone.Stats.Add(pair.Key, (Stat)pair.Value.Clone());
        }
        clone.Mon_Inven = (MonInven)this.Mon_Inven.Clone();
        return clone;
    }
    public void AddToCurPoint(string key, float value)
    {
        if (Stats.ContainsKey(key))
        {
            Stats[key].cur_point += value;
        }
        else if (Cons.n_Cons.ContainsKey(key))
        {
            Cons.n_Cons[key].cur_point += value;
        }
        else if (key == Const.money) Mon_Inven.Money += value;
        else Debug.LogError(key + "에 해당하는 플레이어 데이터가 존재하지 않음");
    }

    public float GetCurPoint(string key)
    {
        if (Stats.ContainsKey(key))
        {
            return Stats[key].cur_point;
        }
        else if (Cons.n_Cons.ContainsKey(key))
        {
            return Cons.n_Cons[key].cur_point;
        }
        else if (key == Const.money) return Mon_Inven.Money;
        else Debug.LogError(key + "에 해당하는 플레이어 데이터가 존재하지 않음");
        return 0;
    }

    public float GetStatPercent(string key)
    {
        if (Stats.ContainsKey(key)) return Stats[key].cur_point / Stats[key].max_point;
        else if (Cons.n_Cons.ContainsKey(key)) return Cons.n_Cons[key].cur_point / Cons.n_Cons[key].max_point;
        else throw new NullReferenceException();
    }

    public string GetDefStat(string key)
    {
        float effectValue=0;
        for(int i=0; i < Mon_Inven.Equipment.Count; i++)
        {
            if (Mon_Inven.Equipment[i].item_name == "none") continue;
            Equipment itemData = (Equipment)DataManager.Instance.GetItemData(Mon_Inven.Equipment[i].item_name);
            for (int j=0; j<itemData.effect.Count; j++)
            {
                if (itemData.effect[j].effect_variable == key) effectValue += itemData.effect[j].GetMinValue();
            }
        }
        return Stats[key].name + ": " + Stats[key].cur_point +
            "[" + (Stats[key].cur_point-effectValue) + "(/" + Stats[key].max_point + ")" +
            string.Format("{0:+0;-0}", effectValue) + "]";
    }

    public string GetBatStat()
    {
        string[] stats = new string[3];

        for(int i=0; i<stats.Length; i++)
        {
            if (i == 1) stats[i] = Stats[Const.batStats[i]].name + " " + Stats[Const.batStats[i]].cur_point.ToString("F2");
            else stats[i] = Stats[Const.batStats[i]].name + " " + Stats[Const.batStats[i]].cur_point;
        }

        return string.Format("{0, -5} {1, -5} {2, -5}", stats[0], stats[1], stats[2]);
    }

    public string GetStateOutOfMax(string key)
    {
        return Cons.n_Cons[key].cur_point + "/" + Cons.n_Cons[key].max_point;
    }

    public string GetMoney()
    {
        return string.Format("{0:#,##0}", Mon_Inven.Money) + "$";
    }

    public void AddInvenItem(string item)   // 장비아이템 인벤토리에 추가 시 호출
    {
        Mon_Inven.Inven = Mon_Inven.Inven.OrderBy(x => x.inven_index).ToList();
        int firstEmptyIndex = 0;
        for (int i = 0; i < Mon_Inven.Inven.Count; i++)
        {
            if (firstEmptyIndex == Mon_Inven.Inven[i].inven_index) firstEmptyIndex++;
        }

        Mon_Inven.Inven.Add(new InvenItem()
        {
            inven_index = firstEmptyIndex,
            item_index = DataManager.Instance.GetItemData(item).data_ID,
            item_name = item
        });
    }

    public void AddInvenItem(string item, int itemEach)   // 소비아이템 인벤토리에 추가 시 호출
    {
        Mon_Inven.Inven = Mon_Inven.Inven.OrderBy(x=>x.inven_index).ToList();
        int firstEmptyIndex = 0;
        int index = -1;
        for(int i=0; i < Mon_Inven.Inven.Count; i++)
        {
            if(Mon_Inven.Inven[i].item_name == item) index = i; // 이미 있는 아이템인지 체크
            if (firstEmptyIndex == Mon_Inven.Inven[i].inven_index) firstEmptyIndex++;

        }

        if (index != -1)
        {
            Mon_Inven.Inven[index].item_each += itemEach;
        }
        else Mon_Inven.Inven.Add(new InvenItem()
        {
            inven_index = firstEmptyIndex,
            item_index = DataManager.Instance.GetItemData(item).data_ID,
            item_name = item,
            item_each = itemEach
        });
    }

    public void AddInvenItemAt(int index, string item)   // 장비아이템 인벤토리의 특정 인덱스에 추가 시 호출
    {
        Mon_Inven.Inven.Add(new InvenItem()
        {
            inven_index = index,
            item_index = DataManager.Instance.GetItemData(item).data_ID,
            item_name = item
        });
    }

    public void AddInvenItemAt(int index, string item, int itemEach)   // 소비아이템 인벤토리의 특정 인덱스에 추가 시 호출
    {
        Mon_Inven.Inven.Add(new InvenItem()
        {
            inven_index = index,
            item_index = DataManager.Instance.GetItemData(item).data_ID,
            item_name = item,
            item_each = itemEach
        });
    }

    public void RemoveInvenItem(PlayerItem item)
    {
        for (int i = 0; i < Mon_Inven.Inven.Count; i++)
        {
            if (((InvenItem)item).inven_index == Mon_Inven.Inven[i].inven_index)
            {
                Mon_Inven.Inven.RemoveAt(i);
            }
        }
    }

    public void RemoveInvenItem(PlayerItem item, int itemEach)
    {
        for(int i=0; i < Mon_Inven.Inven.Count; i++)
        {
            if(((InvenItem)item).inven_index == Mon_Inven.Inven[i].inven_index)
            {
                if (Mon_Inven.Inven[i].item_each == itemEach) Mon_Inven.Inven.RemoveAt(i);
                else Mon_Inven.Inven[i].item_each -= itemEach;
            }
        }
    }

    public void SetEquipItem(int index, string item)
    {
        if (Mon_Inven.Equipment[index].item_name != "none") ResetEquipItem(index);
        ItemData itemData = DataManager.Instance.GetItemData(item);
        for(int i=0; i<itemData.effect.Count; i++)
        {
            AddToCurPoint(itemData.effect[i].effect_variable, itemData.effect[i].GetMinValue());
        }
        Mon_Inven.Equipment[index].item_name = item;
        Mon_Inven.Equipment[index].item_index = DataManager.Instance.GetItemData(item).data_ID;
    }

    public void ResetEquipItem(int index)
    {
        ItemData itemData = DataManager.Instance.GetItemData(Mon_Inven.Equipment[index].item_name);
        for (int i = 0; i < itemData.effect.Count; i++)
        {
            AddToCurPoint(itemData.effect[i].effect_variable, -itemData.effect[i].GetMinValue());
        }
        Mon_Inven.Equipment[index].item_name = "none";
        Mon_Inven.Equipment[index].item_index = 0;
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

public class Con : ICloneable
{
    public Dictionary<string, Stat> n_Cons;
    public List<ConsEtc> ETC;

    public object Clone()
    {
        Con clone = new Con();
        clone.n_Cons = new Dictionary<string, Stat>();
        clone.ETC = new List<ConsEtc>();
        foreach (KeyValuePair<string, Stat> pair in n_Cons)
        {
            clone.n_Cons.Add(pair.Key, pair.Value.Clone() as Stat);
        }
        for(int i=0; i<ETC.Count; i++)
        {
            clone.ETC.Add(this.ETC[i].Clone() as ConsEtc);
        }
        return clone;
    }
}

public class ConsEtc : ICloneable
{
    public string name;
    public int index;

    public object Clone()
    {
        return new ConsEtc
        {
            name = this.name,
            index = this.index
        };
    }
}

public class MonInven : ICloneable
{
    public float Money;
    public List<EquipItem> Equipment;
    public List<InvenItem> Inven;

    public object Clone()
    {
        MonInven monInven = new MonInven();
        monInven.Money = this.Money;
        monInven.Equipment = new List<EquipItem>();
        monInven.Inven = new List<InvenItem>();

        for(int i=0; i < Equipment.Count; i++)
        {
            monInven.Equipment.Add(Equipment[i].Clone() as EquipItem);
        }
        for (int i = 0; i < Inven.Count; i++)
        {
            monInven.Inven.Add(Inven[i].Clone() as InvenItem);
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
            item_index = this.item_index
        };
    }
}

public class InvenItem : PlayerItem, ICloneable
{
    public int inven_index;
    public int item_each;

    public object Clone()
    {
        return new InvenItem
        {
            inven_index = this.inven_index,
            item_each = this.item_each,
            item_name = this.item_name,
            item_index = this.item_index
        };
    }
}

public class PlayerItem
{
    public string item_name;
    //public int item_each;
    public int item_index;
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
        playerData.Mon_Inven.Inven.Clear();
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
