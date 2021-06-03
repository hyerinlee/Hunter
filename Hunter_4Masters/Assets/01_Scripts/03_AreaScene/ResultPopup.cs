using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class ResultPopup : MonoBehaviour
{
    [SerializeField]
    private GameObject[] gauges = new GameObject[4];

    // 프리팹
    private List<string> prefabs = new List<string>();

    private PlayerData beforePd, afterPd;

    private GameObject temp;
    private float duration = 1.0f; // 카운팅에 걸리는 시간 설정.

    private Dictionary<string, bool> changedValues = new Dictionary<string, bool>()
    {
        {Const.sp, false},
        {Const.hp, false},
        {Const.money, false},
        {Const.time, false},
        {Const.defStats[0], false},
        {Const.defStats[1], false},
        {Const.defStats[2], false}
    };

    public void SetPopup(KeyValuePair<string, EventData>[] oea, Dictionary<string, float>[] ca, PlayerData beforePd, PlayerData afterPd)
    {
        WhatAreChanged(oea, ca);    // 변경된 육성데이터 타입 체크

        SetPrefabs();   // 변경된 육성데이터타입을 prefabs에 string으로 저장

        this.beforePd = beforePd;
        this.afterPd = afterPd;

        StartCoroutine(SetGauge());
        for (int i = 0; i < prefabs.Count; i++)
        {
            // 임시 데이터
            PlayAnim(gauges[i], i);
        }

        FosterManager.Instance.SetPlayerData(afterPd);
    }

    IEnumerator SetGauge()
    {
        for(int i=0; i<prefabs.Count; i++)
        {
            temp = (GameObject)Instantiate(Resources.Load("Stats/" + prefabs[i]));
            temp.transform.SetParent(gauges[i].transform);
            temp.transform.localScale = new Vector3(1, 1, 1);
            temp.transform.localPosition = new Vector3(90, 0, 0); //왜 90을 더해줘야 하는걸까 ... ??
        }

        yield return null;
    }

    public void PlayAnim(GameObject obj, int i)
    {
        float target = afterPd.GetCurPoint(prefabs[i]);
        float current = beforePd.GetCurPoint(prefabs[i]);

        if (itIsCount(obj))
        {
            StartCoroutine(PlayCount(obj, target, current));
        }
        else
        {
            StartCoroutine(PlayGauge(obj, target, current, i));
        }

        SetDelta(obj, target-current);
    }

    IEnumerator PlayGauge(GameObject obj, float target, float current, int i)
    {

        temp = obj.transform.GetChild(0).gameObject;
        Image gaugeBar = temp.transform.Find("Gauge").GetComponent<Image>();
        gaugeBar.fillAmount = current / beforePd.GetCurPoint(prefabs[i]); // max

        yield return new WaitForSeconds(2);

        if(target - current < 0)
        {
            float curFillAmount = gaugeBar.fillAmount;

            float offset = (current - target) / duration;
            float max = current;
            current = target;
            while (current < max)
            {
                current += offset * Time.deltaTime;
                gaugeBar.fillAmount = (target / current) - (1 - curFillAmount);
                yield return null;
            }
        }
        else
        {
            float curFillAmount = target / 100;

            float offset = (target - current) / duration;
            while (current < target)
            {
                current += offset * Time.deltaTime;
                gaugeBar.fillAmount = (current / target) - (1 - curFillAmount);
                yield return null;
            }
        }
    }

    IEnumerator PlayCount(GameObject obj, float target, float current)
    {
        temp = obj.transform.GetChild(0).gameObject;
        Text textBox = temp.transform.Find("Gauge").GetComponent<Text>();
        textBox.text = current.ToString();

        yield return new WaitForSeconds(2);

        if(target - current < 0)
        {
            float offset = (current - target) / duration;
            while (current >= target)
            {
                current -= offset * Time.deltaTime;
                if(current < target) {current = target;}
                int tmp = (int)current;
                textBox.text = tmp.ToString();
                yield return null;
            }
        }
        else
        {
            float offset = (target - current) / duration;
            while (current <= target)
            {
                current += offset * Time.deltaTime;
                if(current > target) {current = target;}
                int tmp = (int)current;
                textBox.text = tmp.ToString();
                yield return null;
            }
        }
    }

    private bool itIsCount(GameObject obj)
    {
        if(obj.transform.GetChild(0).gameObject.name == Const.time+"(Clone)" || obj.transform.GetChild(0).gameObject.name == Const.money+ "(Clone)") return true;
        else return false;
    }

    private void SetDelta(GameObject obj, float delta)
    {
        temp = obj.transform.GetChild(0).gameObject;
        Text txt = temp.transform.Find("Delta").GetComponent<Text>();
        int del = (int)delta;
        txt.text = delta.ToString();
    }

    private void WhatAreChanged(KeyValuePair<string, EventData>[] oea, Dictionary<string, float>[] ca)
    {
        for(int i=0; i<oea.Length; i++)
        {
            foreach(var values in ca[i].Keys)
            {
                Debug.Log(i + "번째 딕셔너리 변경 항목 출력 : " + values);
                if(changedValues.ContainsKey(values))
                {
                    changedValues[values] = true;
                }
            }
        }
    }

    private void SetPrefabs() // use changedValues dictionary
    {
        foreach(var key in changedValues.Keys)
        {
            if(changedValues[key])
            {
                prefabs.Add(key);
            }
        }
    }

    public void Exit()
    {
        ResetWhatAreChanged();
        RemoveGaugesChild();
        ResetPrefabList();
        GameManager.Instance.Resume();
    }

    private void ResetWhatAreChanged()
    {
        List<string> keyList = new List<string>(changedValues.Keys);

        foreach (var key in keyList)
        {
            changedValues[key] = false;
        }

        Debug.Log("불린 초기화");
    }

    private void ResetPrefabList()
    {
        this.prefabs.Clear();
        Debug.Log("프리팹 초기화");
    }

    private void RemoveGaugesChild()
    {
         for(int i=0; i<prefabs.Count; i++)
        {
            Destroy(gauges[i].transform.GetChild(0).gameObject);
        }

        Debug.Log("오브젝트 초기화");
    }
}