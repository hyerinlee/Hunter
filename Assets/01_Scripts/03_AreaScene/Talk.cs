using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talk : MonoBehaviour
{
    private State state = State.NotInitialized;

    private List<string> text = new List<string>(new string[]
    {
        "영일이삼사오육칠팔구 영일이삼사오육칠팔구 영일이삼사오육칠팔구 영일이삼사오육칠팔구",
        "두번째 텍스트입니다. 스킵 버튼 터치 시 모든 텍스트가 출력됩니다.",
        "마지막 텍스트입니다. 터치하면 종료합니다."
    });
    
    [SerializeField]
    Text uiText;


    enum State
    {
        NotInitialized,
        Playing,
        PlayingSkipping,
        Completed,
    }

    IEnumerator Start()
    {
        state = State.Playing;
        for (int i = 0; i < text.Count; i += 1)
        {
            yield return PlayLine(text[i]);
        }
        state = State.Completed;
    }

    IEnumerator PlayLine(string text)
    {
        for (int i = 0; i < text.Length + 1; i += 1)
        {
            yield return new WaitForSeconds(0.1f);
            if (state == State.PlayingSkipping)
            {
                uiText.text = text;
                state = State.Playing;
                break;
            }
            uiText.text = text.Substring(0, i);
        }
        
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 25; i += 1)
        {
            yield return new WaitForSeconds(0.1f);
            if (state == State.PlayingSkipping)
            {
                state = State.Playing;
                break;
            }
        }
    }

    public void Skip()
    {
        Debug.Log(state);
        if(state == State.Completed)
        {
            Debug.Log("종료");
            uiText.transform.parent.gameObject.SetActive(false);
        }

        state = State.PlayingSkipping;
    }

    public IEnumerator WaitForComplete()
    {
        while (state != State.Completed)
        {
            yield return null;
        }
    }
}