using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.IO;

// 상수 정의
public static class Constants
{
    public const string Inven = "Inventory";
    public const string Equip = "Equipment";
    public const string consumable = "consumable";

    public static readonly string[] equipType = { "weapon", "armor", "accessory" };
    public static readonly string[] equipTxt = { "무기", "갑옷", "악세" };
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
    public float effect_min;
    public float effect_max;
}

public class ItemData
{
    public int data_ID;
    public string name;
    public string type;
    public List<Condition> condition;
    public List<Effect> effect;

    public string GetItemDescription()
    {
        string res = "요구 스탯:";
        for(int i=0; i<condition.Count; i++)
        {
            if (i > 0) res += ",";
            res += condition[i].condition_variable + " " + condition[i].condition_per;
        }
        res += "\n\n";
        for(int i=0; i < effect.Count; i++)
        {
            res += effect[i].effect_variable + " " + effect[i].effect_min + "\n";
        }
        return res;
    }
}
#endregion

public class DataManager : Singleton<DataManager>
{
    Dictionary<string, ActionData> actionDataDict = new Dictionary<string, ActionData>();    // moveData, laborData, ...
    Dictionary<string, ItemData> itemDataDict = new Dictionary<string,ItemData>();

    public override void Awake()
    {
        base.Awake();
        LoadActionData("Move");
        LoadItemData();
    }

    #region Load Json
    private void LoadActionData(string action)
    {
        ActionData actionData = new ActionData();
        actionData.choiceDataDict = JsonConvert.DeserializeObject<Dictionary<string, ChoiceData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_choice").text);
        actionData.eventDataDict = JsonConvert.DeserializeObject<Dictionary<string, EventData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_event").text);
        actionDataDict.Add(action + "Data", actionData);

        // 행동 단위로 로드할 경우
        //ActionData.Add(action+"Data", JsonConvert.DeserializeObject<Action>(Resources.Load<TextAsset>($"Data/"+action+"Data").text));

    }

    private void LoadItemData()
    {
        itemDataDict = JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(Resources.Load<TextAsset>($"Data/ItemData").text);
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

    public ItemData GetItemData(string name)
    {
        return itemDataDict[name];
    }

    // ex) 780.0 -> 1:00 PM
    public string GetTimeByValue(int value)
    {
        string curTime;
        int t = value / 60 - 12 * (value / 720);
        if (value % 720 < 60) t += 12;    // 12:00am~12:50am or 12:00pm~12:50pm
        curTime = t.ToString("D2") + ":" + (value % 60).ToString("D2");

        return (value >= 720) ?
        curTime += " PM" :
        curTime += " AM";
    }

    // ex) 350.0 -> "+6시간 50분"
    public string GetEstimatedTimeByValue(float value)
    {
        string estTime = "+" + value / 60 + "시간";
        if (value % 60 != 0) estTime += " " + value % 60 + "분";
        return estTime;
    }
}
