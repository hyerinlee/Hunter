using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationPopup : MonoBehaviour
{
    [SerializeField] private Animator simulAnim;
    [SerializeField] private Text[] simulText = new Text[3];

    private int actionIndex;
    private int optionIndex;
    private float eventPercentage;

    private float textScrollSpeed = 0.3f;
    private float textScrollAmount = 66;
    private float textMaxY = 90;

    // 테스트용 임시변수(실제로 사용 X)
    private int eventOccurNum = 38;    // 0010 0110 (2,5,6번째에 실행)
    private string[,] simulTextTempData = { {"...", "강도들이 나타났다.(Hp -20, 재화 -50%)"},
                                        {"...", "돌부리에 걸려 넘어졌다.(Hp -20)" } };

    public void SetPopup(int actIndex, int optIndex)
    {
        actionIndex = actIndex;
        optionIndex = optIndex;
        simulAnim.SetInteger("actionIndex", actionIndex);
        simulAnim.SetInteger("optionIndex", optionIndex);
        StartCoroutine(Simulate());
    }

    IEnumerator Simulate()
    {
        // 현재는 테스트용으로 지정 순서에 이벤트 발생하나, 실제로는 확률 적용해야함
        for(int i = 0; i < 8; i++)
        {
            if ((eventOccurNum & 128 >> i) != 0)
            {
                simulText[i % 3].text = simulTextTempData[actionIndex, 1];
                simulAnim.SetBool("isEventOccur", true);
            }
            else
            {
                simulText[i % 3].text = simulTextTempData[actionIndex, 0];
                simulAnim.SetBool("isEventOccur", false);
            }
            if (i > 1)
            {
                for(int j = 0; j < 3; j++)
                {
                    StartCoroutine(ScrollText(simulText[j]));
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
        // 시뮬레이션 끝난 후 텍스트내용 초기화
        for(int i = 0; i < 2; i++)
        {
            simulText[i].text = "";
        }
        this.gameObject.SetActive(false);
        // resultPopup.SetActive(true);
    }

    IEnumerator ScrollText(Text text)
    {
        float count = 0;
        Vector2 startPos = text.rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * textScrollAmount;

        while (count < textScrollSpeed)
        {
            count += Time.deltaTime;
            text.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, count / textScrollSpeed);
            yield return null;
        }
        text.rectTransform.anchoredPosition = endPos;
        if (text.rectTransform.anchoredPosition.y > textMaxY) text.rectTransform.anchoredPosition += Vector2.down * textScrollAmount*3;
    }
}
