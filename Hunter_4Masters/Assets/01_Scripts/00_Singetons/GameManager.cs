using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int leftTimeVal = 43200;   // 60(분)*24(시간)*30(일)
    public int d_day, curTimeVal;
    float timeScale = 6f;
    int gameTimeScale = 10; // 실제시간 timeScale 초마다 인게임 gameTimeScale 분씩 지남
    float delay = 0.0f;
    IEnumerator timeCoroutine;

    public bool isPlay;
    public int tempSpendTime = 0;  // 시뮬레이션 실행시 소모되는 시간 임시 저장

    private void Start()
    {
        CalcTime();
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
                CalcTime();
            }
            yield return null;
        }
    }

    // 남은 시간으로 남은 일수와 현재시각을 계산 및 값 갱신
    private void CalcTime()
    {
        d_day = (int)Mathf.Ceil((float)leftTimeVal / 1440);
        curTimeVal = (1440 - leftTimeVal % 1440) % 1440;
    }

    public string GetDDay()
    {
        return StatConverter.GetBasicDDay(d_day);
    }

    public string GetCurrentTimeByValue()
    {
        return StatConverter.GetBasicTime(curTimeVal);
        //return DataManager.Instance.GetTimeByValue(curTimeVal);
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
        leftTimeVal += tempSpendTime;
        tempSpendTime = 0;
        CalcTime();
        isPlay = true;
        timeCoroutine = TimeCoroutine();
        StartCoroutine(timeCoroutine);
    }
}
