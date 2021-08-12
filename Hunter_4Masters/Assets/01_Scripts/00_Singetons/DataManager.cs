using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.IO;
using System;

// 상수 정의
public static class Const
{
    public const int etcMaxSize = 10;
    public const int minDef = -10000;
    public const int maxDef = 9999;
    public const int equipNum = 3;
    public const int potionNum = 2;

    public const string defStr = "none";
    public const string defStr2 = "null";  // 통일해야 될듯

    public const string hp = "HP";
    public const string sp = "SP";
    public const string will = "WILL";

    public const string money = "MONEY";
    public const string time = "TIME";

    public const string inven = "Inven";
    public const string equip = "Equipment";

    public static readonly string[] defStats = { "STR", "DEX", "INT" };
    public static readonly string[] batStats = { "ATK", "APS", "DEF" };

    public static readonly string[] equipType = { "weapon", "armor", "accessory", "potion", "potion" };
    public static readonly string[] itemCategory = { "장비-무기", "장비-갑옷", "장비-악세", "포션" };
}

#region Define Data Classes
public class ActionData
{
    public Dictionary<string, ChoiceData> choiceDataDict;  // Title, walk, bus, convoy,...
    public Dictionary<string, EventData> eventDataDict;    // 
}

public class ChoiceData
{
    public int data_ID;
    public string name;
    public int area;
    public string category;
    public List<Consume> consume;
    public string plusInfo;
    public List<Condition> condition;
    public List<Event> events; // 'event'는 에러남
}
public class EventData
{
    public int data_ID;
    public string name;
    public string category;
    public int pre_ID;
    public string mention;
    public List<Condition> condition;
    public List<Effect> effect;
}

public class Consume
{
    public string consume_variable;
    public float consume_value;
}

public class Condition
{
    public string condition_variable;
    public float condition_min;
    public float condition_max;
    public string condition_per;
}
public class Event
{
    public int event_ID;
    public float event_per;
}
public class Effect
{
    public string effect_variable;
    public string effect_min;
    public string effect_max;

    public float GetMinValue()
    {
        return StringCalculator.Calculate(effect_min);
    }
    public float GetMaxValue()
    {
        return StringCalculator.Calculate(effect_max);
    }
}

public class Equipment : ItemData
{
    public List<Condition> condition;
    public int spEffectIndex;
    public int skillIndex;
}

public class Potion : ItemData
{
    public int max_capacity;
}

public class ItemData
{
    public int data_ID;
    public string name;
    public int category;
    public string rank;
    public float price;
    public List<Effect> effect;

    public string GetItemName()
    {
        string description = "<size=30>" + name + "</size>" + "\n" + "<color=#85a763>" + Const.itemCategory[category] + "</color>\n";
        return description;
    }
}
#endregion

public class DataManager : Singleton<DataManager>
{
    Dictionary<string, ActionData> actionDataDict = new Dictionary<string, ActionData>();    // moveData, laborData, ...
    Dictionary<string, Equipment> equipmentDict = new Dictionary<string, Equipment>();
    Dictionary<string, Potion> potionDict = new Dictionary<string, Potion>();
    Dictionary<int, string> detailMsg = new Dictionary<int, string>()
        {
            {300001, "[빈곤] \n 돈이 없다... \n 근력 * 125% \n 민첩 * 125% \n 재화 획득량 * 125%"},
            {300003, "[좌절] \n 하...이번 생은 망했어 \n 모든 증가 스탯 * 50% \n 모든 감소 스탯 * 200%"},
            {300000, "[영양실조] \n 공격속도 * 50% \n 의지 * 75%"}
        };

    public override void Awake()
    {
        base.Awake();
        LoadActionData("Move");
        LoadActionData("Training");
        LoadItemData();
    }

    #region Load Json
    private void LoadActionData(string action)
    {
        ActionData actionData = new ActionData();

        Dictionary<string, Dictionary<string, ChoiceData>> cdd = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ChoiceData>>>(Resources.Load<TextAsset>($"Data/" + action + "Data_choice").text);
        actionData.choiceDataDict = cdd.Values.ElementAt(0);
        Dictionary<string, Dictionary<string, EventData>> edd = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, EventData>>>(Resources.Load<TextAsset>($"Data/" + action + "Data_event").text);
        actionData.eventDataDict = edd.Values.ElementAt(0);

        actionDataDict.Add(action + "Data", actionData);

        // 행동 단위로 로드할 경우
        //ActionData.Add(action+"Data", JsonConvert.DeserializeObject<Action>(Resources.Load<TextAsset>($"Data/"+action+"Data").text));

    }

    private void LoadItemData()
    {
        equipmentDict = JsonConvert.DeserializeObject<Dictionary<string, Equipment>>(Resources.Load<TextAsset>($"Data/equipment").text);
        potionDict = JsonConvert.DeserializeObject<Dictionary<string, Potion>>(Resources.Load<TextAsset>($"Data/potion").text);
    }
    #endregion

    public ChoiceData GetChoiceTitle(string action)
    {
        return actionDataDict[action + "Data"].choiceDataDict[action + "_Title"];
    }

    public Dictionary<string, ChoiceData> GetChoiceData(string action, int areaCode)
    {
        Dictionary<string, ChoiceData> choiceDataDict = new Dictionary<string, ChoiceData>();

        // key값이 action + "_" + areaCode + "_"를 포함하는 밸류를 다 가져오기...
        foreach (KeyValuePair<string, ChoiceData> pair in actionDataDict[action + "Data"].choiceDataDict)
        {
            if (pair.Key.Contains(action + "_" + areaCode + "_"))
            {
                choiceDataDict.Add(pair.Key, pair.Value);
            }
        }

        return choiceDataDict;
    }

    public KeyValuePair<string, EventData> GetEventDataById(string action, int id)
    {
        foreach (KeyValuePair<string, EventData> pair in actionDataDict[action + "Data"].eventDataDict)
        {
            if (pair.Value.data_ID == id)
            {
                return new KeyValuePair<string, EventData>(pair.Key, pair.Value);
            }
        }
        return new KeyValuePair<string, EventData>(null, null);
    }

    public List<KeyValuePair<string, EventData>> GetSecondaryEventList(string action, int primaryId)
    {
        List<KeyValuePair<string, EventData>> secondaryEventData = new List<KeyValuePair<string, EventData>>();

        foreach (KeyValuePair<string, EventData> pair in actionDataDict[action + "Data"].eventDataDict)
        {
            if (pair.Value.pre_ID == primaryId)
            {
                secondaryEventData.Add(pair);
            }
        }

        return secondaryEventData;
    }

    public string GetKey(ItemData id)
    {
        foreach(KeyValuePair<string, Equipment> pair in equipmentDict)
        {
            if (pair.Value == id) return pair.Key;
        }

        foreach (KeyValuePair<string, Potion> pair in potionDict)
        {
            if (pair.Value == id) return pair.Key;
        }

        throw new NullReferenceException();
    }

    public ItemData GetItemData(PlayerItem item)
    {
        if (equipmentDict.ContainsKey(item.item_name)) return equipmentDict[item.item_name];
        else if (potionDict.ContainsKey(item.item_name)) return potionDict[item.item_name];
        else throw new NullReferenceException();
    }

    public ItemData GetItemData(string name)
    {
        if (equipmentDict.ContainsKey(name)) return equipmentDict[name];
        else if (potionDict.ContainsKey(name)) return potionDict[name];
        else throw new NullReferenceException();
    }

    public Dictionary<string, Equipment> GetAllEquipments()
    {
        return equipmentDict;
    }

    public Dictionary<string, Potion> GetAllPotions()
    {
        return potionDict;
    }

    // 임시
    public string GetEtcDetail(int index)
    {
        return detailMsg[index];
    }

    // ResourceManager 구현 후 교체 예정
    public Sprite GetSprite(string folder, string name)
    {
        if(folder=="") return Resources.Load(name, typeof(Sprite)) as Sprite;
        else return Resources.Load(folder + "/" + name, typeof(Sprite)) as Sprite;
    }
}
