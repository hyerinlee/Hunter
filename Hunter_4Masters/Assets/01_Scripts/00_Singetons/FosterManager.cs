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
        else if (key == Const.time) GameManager.Instance.tempSpendTime += (int)value;
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
        else if (key == Const.time) return GameManager.Instance.leftTimeVal;
        else if (key == Const.money) return Mon_Inven.Money;
        else Debug.LogError(key + "에 해당하는 플레이어 데이터가 존재하지 않음");
        return 0;
    }

    public float GetStatMax(string key)
    {
        if (Stats.ContainsKey(key)) return Stats[key].max_point;
        else if (Cons.n_Cons.ContainsKey(key)) return Cons.n_Cons[key].max_point;
        else throw new NullReferenceException();
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
        for(int i=0; i < Const.equipNum; i++)
        {
            if (Mon_Inven.Equipment[i].item_name == "none") continue;
            Equipment itemData = (Equipment)DataManager.Instance.GetItemData(Mon_Inven.Equipment[i]);
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
        return StatConverter.GetMoney(Mon_Inven.Money);
    }

    public PlayerItem FindItemWithIndex(int slotIndex)
    {
        for(int i=0; i<Mon_Inven.Inven.Count; i++)
        {
            if (Mon_Inven.Inven[i].inven_index == slotIndex) return Mon_Inven.Inven[i];
        }
        return null;
    }

    // 아이템 인벤토리에 추가 시 호출(ItemData로)
    public void AddInvenItem(ItemData item, int index = -1, int itemEach = -1)
    {
        if (item == null || itemEach == 0) return;

        if (index == -1)    // 사용자 지정 인덱스가 아닐 경우 적절한 인덱스 탐색
        {
            Mon_Inven.Inven = Mon_Inven.Inven.OrderBy(x => x.inven_index).ToList();

            if (itemEach == -1)
            {   // 장비아이템일 때
                index++;

                for (int i = 0; i < Mon_Inven.Inven.Count; i++)
                {
                    if (index == Mon_Inven.Inven[i].inven_index) index++;   // 리스트 인덱스와 인벤 인덱스가 불일치하는 구간이 가장 처음 비는 인덱스
                }
            }
            else
            {   // 포션아이템일 때
                int firstEmptyIndex = 0;
                for (int i = 0; i < Mon_Inven.Inven.Count; i++)
                {
                    if (Mon_Inven.Inven[i].item_index == item.data_ID) index = i; // 이미 있는 아이템인지 체크
                    if (firstEmptyIndex == Mon_Inven.Inven[i].inven_index) firstEmptyIndex++;

                }

                if (index != -1)
                {
                    Mon_Inven.Inven[index].item_each += itemEach;
                    return;
                }
                else index = firstEmptyIndex;
            }
        }

        Mon_Inven.Inven.Add(new InvenItem()
        {
            inven_index = index,
            item_index = item.data_ID,
            item_name = DataManager.Instance.GetKey(item)
        });

        if (itemEach > 0) FindItemWithIndex(index).item_each = itemEach; // 포션아이템일 경우 개수 추가
    }

    // 아이템 인벤토리에 추가 시 호출(PlayerItem으로)
    public void AddInvenItem(PlayerItem item, int index = -1, int itemEach = -1)
    {
        if (item == null || itemEach == 0) return;
        AddInvenItem(DataManager.Instance.GetItemData(item), index, itemEach);
    }


    public void RemoveInvenItem(PlayerItem item)
    {
        if (item == null) return;
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
        if (item == null) return;
        for (int i=0; i < Mon_Inven.Inven.Count; i++)
        {
            if(((InvenItem)item).inven_index == Mon_Inven.Inven[i].inven_index)
            {
                if (Mon_Inven.Inven[i].item_each == itemEach) Mon_Inven.Inven.RemoveAt(i);
                else Mon_Inven.Inven[i].item_each -= itemEach;
            }
        }
    }

    public void SetEquipItem(int index, PlayerItem item)
    {
        ResetEquipItem(index);
        if (item != null)
        {
            ItemData itemData = DataManager.Instance.GetItemData(item);
            for (int i = 0; i < itemData.effect.Count; i++)
            {
                AddToCurPoint(itemData.effect[i].effect_variable, itemData.effect[i].GetMinValue());
            }
            Mon_Inven.Equipment[index].item_name = item.item_name;
            Mon_Inven.Equipment[index].item_index = item.item_index;
            if (itemData.category==3) Mon_Inven.Equipment[index].item_each = item.item_each;
        }
    }

    // 기존 장비의 장착을 해제하고, 장착 효과를 없앰
    public void ResetEquipItem(int index)
    {
        if (Mon_Inven.Equipment[index].item_name != Const.defStr)   // 해당 인덱스에 아이템이 저장되어 있는 경우
        {
            ItemData itemData = DataManager.Instance.GetItemData(Mon_Inven.Equipment[index]);
            for (int i = 0; i < itemData.effect.Count; i++)
            {
                AddToCurPoint(itemData.effect[i].effect_variable, -itemData.effect[i].GetMinValue());
            }
            Mon_Inven.Equipment[index].item_name = "none";
            Mon_Inven.Equipment[index].item_index = 0;
        }
    }

    //public void RemoveEquipItem(EquipItem item)
    //{
    //    Mon_Inven.Equipment.RemoveAt(item.equip_index);
    //}

    public void RemoveEquipItem(EquipItem item, int itemEach)
    {
        if (Mon_Inven.Equipment[item.equip_index].item_each == itemEach) ResetEquipItem(item.equip_index);
        else Mon_Inven.Equipment[item.equip_index].item_each -= itemEach;
    }

    // (장비/인벤 공통) 동일 포션을 병합: item 개수를 itemEach만큼 더하고 남는 개수를 리턴
    public int AddPotion(ref PlayerItem item, int itemEach)
    {
        int surplus = item.item_each + itemEach - ((Potion)DataManager.Instance.GetItemData(item)).max_capacity;

        item.item_each = Mathf.Clamp(item.item_each + itemEach, 0, ((Potion)DataManager.Instance.GetItemData(item)).max_capacity);

        return (surplus > 0) ? surplus : 0;
    }

    public void UsePotion(EquipItem item)
    {
        // item_each 하나 감소시키는데, 만약 감소후 0이면 버튼이미지랑 갯수오브젝트 setactive false 해야됨.
        RemoveEquipItem(item, 1);
        for (int i = 0; i < DataManager.Instance.GetItemData(item).effect.Count; i++)
        {

        }
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
            item_each = this.item_each,
            item_name = this.item_name,
            item_index = this.item_index
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
            item_each = this.item_each,
            item_name = this.item_name,
            item_index = this.item_index
        };
    }
}

public class PlayerItem
{
    public string item_name;
    public int item_each;
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
