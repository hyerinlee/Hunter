using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SimulationPopup : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    private ResultPopup resultPopup;

    [SerializeField] private Animator simulAnimator;
    [SerializeField] private Text[] simulText = new Text[3];

    private float textScrollSpeed = 0.3f;
    private float textScrollAmount = 66;
    private float textMaxY = 90;

    private void Start()
    {
        resultPopup = resultPanel.GetComponent<ResultPopup>();
    }

    public void Simulate(KeyValuePair<string, EventData>[] oea, Dictionary<string, float>[] ca, PlayerData beforePd, PlayerData afterPd)
    {
        StartCoroutine(SimulateCoroutine(oea, ca, beforePd, afterPd));
    }

    IEnumerator SimulateCoroutine(KeyValuePair<string, EventData>[] oea, Dictionary<string, float>[] ca, PlayerData beforePd, PlayerData afterPd)
    {
        for (int i = 0; i < oea.Length; i++)
        {
            simulAnimator.Play(oea[i].Key);

            string changeMention = "";
            int cnt = 0;
            foreach (KeyValuePair<string, float> pair in ca[i])
            {
                if (pair.Key == "null") continue;
                else if (cnt > 0) changeMention += ", ";
                changeMention += pair.Key + " " + string.Format("{0:+0;-0}", pair.Value);
                cnt++;
            }
            if (changeMention != "") changeMention = " (" + changeMention + ")";
            simulText[i % 3].text = oea[i].Value.mention + changeMention;

            if (i > 1)
            {
                for (int j = 0; j < 3; j++)
                {
                    StartCoroutine(ScrollText(simulText[j]));
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
        yield return null;

        // 시뮬레이션 끝난 후 텍스트내용 초기화
        for (int i = 0; i < 3; i++)
        {
            simulText[i].text = "";
        }
        this.gameObject.SetActive(false);
        resultPanel.SetActive(true);
        resultPopup.SetPopup(oea, ca, beforePd, afterPd);
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
        if (text.rectTransform.anchoredPosition.y > textMaxY) text.rectTransform.anchoredPosition += Vector2.down * textScrollAmount * 3;
        yield return null;
    }
}
