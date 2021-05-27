using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.IO;

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
    public List<Consume> consume;  // List로 동적할당
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
    public string name;
}
#endregion

public class DataManager : Singleton<DataManager>
{
    Dictionary<string, ActionData> actionDataDict = new Dictionary<string, ActionData>();    // moveData, laborData, ...

    private void Start()
    {
        LoadActionData("Move");

    }

    #region Load Json
    private void LoadActionData(string action)
    {
        // 선택지, 이벤트 json 파일 분리되어있을 경우
        ActionData actionData = new ActionData();
        actionData.choiceDataDict = JsonConvert.DeserializeObject<Dictionary<string, ChoiceData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_choice").text);
        actionData.eventDataDict = JsonConvert.DeserializeObject<Dictionary<string, EventData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_event").text);
        actionDataDict.Add(action + "Data", actionData);

        //actionDataDict[action + "Data"].choiceDataDict =
        //   JsonConvert.DeserializeObject<Dictionary<string, ChoiceData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_choice").text);
        //actionDataDict[action + "Data"].eventDataDict =
        //    JsonConvert.DeserializeObject<Dictionary<string, EventData>>(Resources.Load<TextAsset>($"Data/" + action + "Data_event").text);

        // 행동 단위로 로드할 경우(이게 더 깔끔한 것 같긴 한데 'json에서의 key값'=='클래스에서의 변수명' 이어야 한다는점이 쪼꼼...)
        //ActionData.Add(action+"Data", JsonConvert.DeserializeObject<Action>(Resources.Load<TextAsset>($"Data/"+action+"Data").text));

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

    //ex)             "00_move_w_accident"                     "Move"    "Move_0_walk"
    //public Dictionary<string, EventData> GetEventData(string action, string option)
    //{
    //    Dictionary<string, EventData> optionEventsDict = new Dictionary<string, EventData>();

    //    for (int i = 0; i < actionDataDict[action + "Data"].choiceDataDict[option].events.Count; i++)
    //    {
    //        KeyValuePair<string, EventData> pair = GetEventDataById(action, actionDataDict[action + "Data"].choiceDataDict[option].events[i].event_ID);
    //        optionEventsDict.Add(pair.Key, pair.Value);
    //    }

    //    // ID말고 key값으로 찾을수도 있음(더 간단하지만, 딕셔너리형에서 리스트형으로 바뀔경우에는 사용 못함)
    //    //foreach (KeyValuePair<string, EventData> pair in actionDataDict[action + "Data"].eventDataDict)
    //    //{
    //    //    if (pair.Key.Contains(action + "_" + option[0]))
    //    //    {
    //    //        optionEventsDict.Add(pair.Key, pair.Value);
    //    //    }
    //    //}

    //    return optionEventsDict;
    //}

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

    // ex) 780.0 -> 1:00 PM
    public string GetTimeByValue(int value)
    {
        return (value >= 720) ?
        (value / 60 - 12).ToString("D2") + ":" + (value % 60).ToString("D2") + " PM" :
        (value / 60).ToString("D2") + ":" + (value % 60).ToString("D2") + " AM";

    }

    // ex) 350.0 -> "6시간 50분" 근데이거 데이터매니저에 있을건 아닌거같은데...
    public string GetEstimatedTimeByValue(float value)
    {
        return (value % 60 == 0) ? value / 60 + "시간" : value / 60 + "시간 " + value % 60 + "분";
    }
}
