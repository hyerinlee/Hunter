using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talk : MonoBehaviour
{
    // 현재는 텍스트 파일을 직접 추가하는 방식임 Resource 추출로 바꿔야 함
    public TextAsset txt;

    string[,] Sentence;
    int lineSize, rowSize;

    private State state = State.NotInitialized;
    private List<string> script = new List<string>();

    public int currentIdx = 0;
    
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
        // 엔터단위와 탭으로 나눠서 배열의 크기 조정
        string currentText = txt.text.Substring(0, txt.text.Length-1);
        string[] line = currentText.Split('\n');

        // 문장 수 계산
        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;
        Sentence = new string[lineSize, rowSize];

        // //한 줄에서 탭으로 나눔
        // for(int i=0; i<lineSize; i++)
        // {
        //     string[] row = line[i].Split('\t');
        //     for(int j=0; j<rowSize; j++)
        //         Sentence[i, j] = row[j];
        // }

        // print(currentText);
        // print(Sentence);

        //script = line.ToList();

        // line 배열을 script 리스트에 추가
        script.AddRange(line);

        // // script 리스트 출력
        // for(int i=0; i<lineSize; i++)
        //     Debug.Log(script[i]);

        StartCoroutine(Play());
    }

    // public void TestStart()
    // {
    //     // 엔터단위와 탭으로 나눠서 배열의 크기 조정
    //     string currentText = txt.text.Substring(0, txt.text.Length-1);
    //     string[] line = currentText.Split('\n');

    //     // 문장 수 계산
    //     lineSize = line.Length;
    //     rowSize = line[0].Split('\t').Length;
    //     Sentence = new string[lineSize, rowSize];

    //     // //한 줄에서 탭으로 나눔
    //     // for(int i=0; i<lineSize; i++)
    //     // {
    //     //     string[] row = line[i].Split('\t');
    //     //     for(int j=0; j<rowSize; j++)
    //     //         Sentence[i, j] = row[j];
    //     // }

    //     // print(currentText);
    //     // print(Sentence);

    //     //script = line.ToList();

    //     // line 배열을 script 리스트에 추가
    //     script.AddRange(line);

    //     // // script 리스트 출력
    //     // for(int i=0; i<lineSize; i++)
    //     //     Debug.Log(script[i]);

    //     StartCoroutine(Play());
    // }

    IEnumerator Play()
    {
        state = State.Playing;
        for (currentIdx = 0; currentIdx < script.Count; currentIdx++)
        {
            Debug.Log(currentIdx + "번째 줄 출력 중. 내용 : " + script[currentIdx]);

            yield return Print(script[currentIdx]);
        }
        state = State.Completed;

        uiText.text = "";
        uiText.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator Print(string script)
    {
        for (int i = 0; i < script.Length + 1; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (state == State.PlayingSkipping)
            {
                uiText.text = script;
                state = State.Playing;
                break;
            }
            uiText.text = script.Substring(0, i);
        }
        
        while(state != State.PlayingSkipping)
            yield return new WaitForSeconds(0.1f);
        state = State.Playing;
    }

    public void Skip()
    {
        state = State.PlayingSkipping;
    }

    public void Exit()
    {
        uiText.text = "";
        uiText.transform.parent.gameObject.SetActive(false);
    }
}