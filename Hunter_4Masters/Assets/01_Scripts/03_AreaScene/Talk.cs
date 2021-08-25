using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talk : MonoBehaviour
{
    private State state = State.NotInitialized;

    private List<string> text = new List<string>(new string[]
    {
        "첫번째 텍스트입니다. 자동으로 출력됩니다.",
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
    
    void OnEnable()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        state = State.Playing;
        for (int i = 0; i < text.Count; i++)
        {
            yield return Print(text[i]);
        }
        state = State.Completed;

        uiText.text = "";
        uiText.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator Print(string text)
    {
        for (int i = 0; i < text.Length + 1; i++)
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
        
        while(state != State.PlayingSkipping)
            yield return new WaitForSeconds(0.1f);
        state = State.Playing;
    }

    public void Skip()
    {
        state = State.PlayingSkipping;
    }
}