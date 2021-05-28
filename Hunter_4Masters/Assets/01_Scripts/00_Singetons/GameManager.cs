using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int leftTimeVal = 43200;   // 60(분)*24(시간)*30(일)
    public int d_day, curTimeVal;
    float timeScale = 5f;
    int gameTimeScale = 10; // 실제시간 timeScale 초마다 인게임 gameTimeScale 분씩 지남
    float delay = 0.0f;
    IEnumerator timeCoroutine;

    public bool isPlay;

    private void Start()
    {
        d_day = (int)Mathf.Ceil((float)leftTimeVal / 1440);
        curTimeVal = (1440 - leftTimeVal % 1440) % 1440;
        Resume();
    }

    IEnumerator TimeCoroutine()
    {
        while (true)
        {
            delay += Time.deltaTime;
            if (delay >= timeScale)
            {
                leftTimeVal -= gameTimeScale;
                delay = 0.0f;
                d_day = (int)Mathf.Ceil((float)leftTimeVal / 1440);
                curTimeVal = (1440 - leftTimeVal % 1440)%1440;
            }
            yield return null;
        }
    }

    public string GetDDay()
    {
        return "D" + string.Format("{0:-0;+0}", d_day);
    }

    public string GetCurrentTimeByValue()
    {
        return DataManager.Instance.GetTimeByValue(curTimeVal);
    }

    public void Pause()
    {
        isPlay = false;
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
    }

    public void Resume()
    {
        isPlay = true;
        timeCoroutine = TimeCoroutine();
        StartCoroutine(timeCoroutine);
    }
}
