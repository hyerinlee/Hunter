using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPopup : MonoBehaviour
{
    [SerializeField] private GameObject simulationPanel;
    private SimulationPopup simulationPopup;

    // 실행 행동 UI
    [SerializeField] private Image actionImg;
    [SerializeField] private Text actionInfo;

    // 육성 UI
    [SerializeField] private Image spGauge;
    [SerializeField] private Text spTxt;
    [SerializeField] private Text dayTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text moneyTxt;

    // 카테고리 및 선택지 UI
    [SerializeField] private Text[] categories = new Text[5];
    [SerializeField] private GameObject scrollView;
    private GameObject optionGroup;
    private GameObject[] options = new GameObject[10];
    private RectTransform optionGroupRT;
    private float scrollViewHeight;
    private float optionBtnHeight;


    public Sprite[] actionImgTempData = new Sprite[2];
    private string[] actionInfoTempData = { "평화로운 산골마을에 위치한 지리산은 가는 길은 그다지 평화롭지 않을 수도 있습니다.",
                                        "헌터협회 사무보조 인력 모집 중\n\n" +
                                        "근무시간: 시간협의\n" +
                                        "제출서류: 이력서" };
    private PlayerData pd;
    private ChoiceData title;
    private Dictionary<string, ChoiceData> cd;

    // 행동 선택, 선택지 선택 시 값이 할당됨
    private string executedAction;
    private string selectedOption;

    // SimulationPopup에 보내는 데이터. 모두 8개씩 들어감
    private static int eventNum = 8;
    private KeyValuePair<string, EventData>[] occurEventArray = new KeyValuePair<string, EventData>[eventNum];
    private Dictionary<string, float>[] changesArray = new Dictionary<string, float>[eventNum];    // 변화량 합산할때도 사용

    private void Awake()
    {
        simulationPopup = simulationPanel.GetComponent<SimulationPopup>();
        optionGroup = scrollView.transform.GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < optionGroup.transform.childCount; i++)
        {
            options[i] = optionGroup.transform.GetChild(i).gameObject;
        }
        optionGroupRT = optionGroup.GetComponent<RectTransform>();
        // 선택지 버튼의 height값 가져오기(이후 선택지 개수에 따라 선택지박스 height 조정됨)
        scrollViewHeight = scrollView.GetComponent<RectTransform>().rect.height;
        optionBtnHeight = options[0].GetComponent<RectTransform>().rect.height;
    }

    public void SetPopup(string action)
    {
        this.executedAction = action;

        pd = FosterManager.Instance.GetPlayerData();
        title = DataManager.Instance.GetChoiceTitle(action);
        cd = DataManager.Instance.GetChoiceData(action, 0);  //일단 areaCode 0으로 넣음


        // 행동이미지와 행동설명데이터 UI에 적용 (데이터 만들면 변경예정)
        actionImg.sprite = actionImgTempData[0];
        actionInfo.text = actionInfoTempData[0];

        // 육성데이터(pd) UI 적용
        spGauge.fillAmount = pd.Cons["SP"].cur_point / pd.Cons["SP"].max_point;
        spTxt.text = pd.GetStateOutOfMax("SP");
        dayTxt.text = GameManager.Instance.GetDDay();
        timeTxt.text = GameManager.Instance.GetCurrentTimeByValue();
        moneyTxt.text = pd.GetMoney();

        // 카테고리 데이터 UI 적용
        categories[0].text = title.name;
        for (int i = 0; i < 3; i++)
        {
            categories[i + 1].text = title.consume[i].consume_variable;
        }
        categories[4].text = title.plusInfo;

        // 선택지박스 스크롤높이 & 선택지 데이터 UI 적용
        optionGroupRT.offsetMin = new Vector2(optionGroupRT.offsetMin.x, ((scrollViewHeight/optionBtnHeight) - cd.Count) * optionBtnHeight);
        int index = 0;
        foreach (KeyValuePair<string, ChoiceData> pair in cd)
        {
            SetOptionUI(pair, index);
            index++;
        }
        for(int i=0; i<cd.Count; i++)
        {
            options[i].SetActive(true);
        }

    }

    // (하나의) 선택지버튼 UI를 세팅
    private void SetOptionUI(KeyValuePair<string, ChoiceData> pair, int index)
    {
        options[index].transform.GetChild(0).GetComponent<Text>().text = pair.Value.name;
        for (int i = 0; i < 3; i++)
        {
            Text optionTxt = options[index].transform.GetChild(i + 1).GetComponent<Text>();
            // 돈과 시간만 형식이 지정되어있고 나머지는 단순 값 출력
            switch (categories[i + 1].text)
            {
                case "money":
                    optionTxt.text = pair.Value.consume[i].consume_value.ToString() + "$";
                    break;
                case "time":
                    optionTxt.text = DataManager.Instance.GetEstimatedTimeByValue(pair.Value.consume[i].consume_value);
                    break;
                default:
                    optionTxt.text = pair.Value.consume[i].consume_value.ToString();
                    break;
            }
        }
        options[index].transform.GetChild(4).GetComponent<Text>().text = pair.Value.plusInfo;
        options[index].GetComponent<Button>().onClick.AddListener(() => OptionClick(pair.Key));
        options[index].GetComponent<Button>().interactable = IsOptionAvailable(pair.Value);
    }

    // 육성데이터와 비교하여 선택가능한 선택지인지 확인
    private bool IsOptionAvailable(ChoiceData choiceData)
    {
        for (int i = 0; i < choiceData.consume.Count; i++)
        {
            string variable = choiceData.consume[i].consume_variable;
            if (variable == "time")
            {
                if (GameManager.Instance.curTimeVal + choiceData.consume[i].consume_value >= 1440) return false; // 실행 후 24시가 넘어간다면 선택불가
            }
            else if (pd.GetCurPointOfAllType(variable) + choiceData.consume[i].consume_value < 0) return false;
        }

        return true;
    }

    public void OptionClick(string option)
    {
        this.selectedOption = option;
        MakeOccurEvents();
        this.gameObject.SetActive(false);
        simulationPanel.SetActive(true);
        simulationPopup.Simulate(occurEventArray, changesArray, pd, GetAfterPd());
        // 선택지 리셋
        for(int i=0; i<cd[option].events.Count; i++)
        {
            options[i].GetComponent<Button>().onClick.RemoveAllListeners();
            options[i].SetActive(false);
        }
    }

    private void MakeOccurEvents()
    {
        List<KeyValuePair<string, EventData>>[] secondaryEventData = new List<KeyValuePair<string, EventData>>[cd[selectedOption].events.Count];
        // 현재 육성데이터 수치와 선택지에 따라서 발생하게 될 이벤트(8개)를 가중치 랜덤값으로 결정하고
        // 해당 이벤트에서 발생할 특정요소의 변화량(랜덤) 결정(ex: 무장강도, hp -20)

        // 각 이벤트에 대한 가중치 계산 & 2차이벤트 저장
        float weightPer = 0;
        List<float> weightNum = new List<float>();
        for (int i = 0; i < cd[selectedOption].events.Count; i++)
        {
            weightPer += cd[selectedOption].events[i].event_per;
            weightNum.Add(weightPer);
            Debug.Log(cd[selectedOption].events[i].event_ID + "=" + weightPer + ", ");
            secondaryEventData[i] = DataManager.Instance.GetSecondaryEventList(executedAction, cd[selectedOption].events[i].event_ID);
        }

        // 발생 이벤트 랜덤생성

        for (int i = 0; i < eventNum; i++)
        {
            float randomNum = Random.Range(0, 100.1f);
            int randomIndex = 0;

            // 랜덤숫자가 속하는 인덱스의 이벤트 저장(ex: '걷기' 선택지에서 랜덤값이 30이 나왔을 경우 2번째 이벤트 발생함)
            for (int j = 0; j < weightNum.Count; j++)
            {
                if (randomNum <= weightNum[j])
                {
                    randomIndex = j;
                    break;
                }
            }

            occurEventArray[i] = MakeSecondaryOccurEvent(DataManager.Instance.GetEventDataById(executedAction, cd[selectedOption].events[randomIndex].event_ID), secondaryEventData[randomIndex]);

            // 해당 이벤트에서 발생할 특정요소의 변화량(랜덤) 결정(ex: 무장강도, hp -20)
            Dictionary<string, float> changes = new Dictionary<string, float>();
            for (int j = 0; j < occurEventArray[i].Value.effect.Count; j++)
            {
                int changeAmount = (int)(Random.Range(occurEventArray[i].Value.effect[j].effect_min, occurEventArray[i].Value.effect[j].effect_max));

                changes.Add(occurEventArray[i].Value.effect[j].effect_variable, changeAmount);
                changesArray[i] = changes;
            }

        }
    }

    // 이벤트에서 발생하는 2차 이벤트 결정
    private KeyValuePair<string, EventData> MakeSecondaryOccurEvent(KeyValuePair<string, EventData> primaryData, List<KeyValuePair<string, EventData>> secondaryEventData)
    {
        // 각 이벤트에 대한 가중치 계산
        float weightPer = 0;
        List<float> weightNum = new List<float>();
        for (int i = 0; i < secondaryEventData.Count; i++)
        {
            if (!IsEventAvailable(secondaryEventData[i].Value)) continue;
            // string형 확률을 float값으로 전환 (육성데이터값 대입)
            for (int j = 0; j < secondaryEventData[i].Value.condition.Count; j++)
            {
                weightPer += StringCalculator.Calculate(secondaryEventData[i].Value.condition[j].condition_per);
            }
            weightNum.Add(weightPer);
            Debug.Log(secondaryEventData[i].Value.data_ID + "=" + weightPer + ", ");
        }

        float randomNum = Random.Range(0, 100.1f);
        int randomIndex = -1;

        // 랜덤숫자가 속하는 2차이벤트의 인덱스 찾기(없으면 1차데이터 리턴)
        for (int i = 0; i < weightNum.Count; i++)
        {
            if (randomNum <= weightNum[i])
            {
                randomIndex = i;
                break;
            }
        }
        return (randomIndex == -1) ? primaryData : secondaryEventData[randomIndex];
    }

    // 육성데이터와 비교하여 이벤트가 발생가능한지 체크
    private bool IsEventAvailable(EventData eventData)
    {
        for (int i = 0; i < eventData.condition.Count; i++)
        {
            string variable = eventData.condition[i].condition_variable;
            // variable이 null이 아니고 조건 범위에 속하지 않으면 return false
            if (variable != "null" && (pd.GetCurPointOfAllType(variable) < eventData.condition[i].condition_min ||
                pd.GetCurPointOfAllType(variable) > eventData.condition[i].condition_max)) return false;
        }
        return true;
    }

    private PlayerData GetAfterPd()
    {
        PlayerData afterPd = pd.Clone() as PlayerData;

        // 선택지로 인해 변화한 값 적용
        for (int i = 0; i < cd[selectedOption].consume.Count; i++)
        {
            if(cd[selectedOption].consume[i].consume_variable == "time")
            {
                GameManager.Instance.leftTimeVal -= (int)cd[selectedOption].consume[i].consume_value;
            }
            else afterPd.AddToCurPointOfAllType(cd[selectedOption].consume[i].consume_variable, cd[selectedOption].consume[i].consume_value);
        }

        // 이벤트로 인해 변화한 값 적용
        for (int i = 0; i < eventNum; i++)
        {
            foreach (KeyValuePair<string, float> pair in changesArray[i])
            {
                if (pair.Key != "null")
                {
                    afterPd.AddToCurPointOfAllType(pair.Key, pair.Value);
                    if (afterPd.GetCurPointOfAllType(pair.Key) < 0) afterPd.AddToCurPointOfAllType(pair.Key, -afterPd.GetCurPointOfAllType(pair.Key));
                }
            }
        }
        return afterPd;
    }
}
