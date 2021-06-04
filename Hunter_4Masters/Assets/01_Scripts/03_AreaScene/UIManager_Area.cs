using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager_Area : Singleton<UIManager_Area>
{
    [SerializeField] private Image hpGauge, spGauge;
    [SerializeField] private GameObject interaction, dialogBox, etcPanel, etcDetail;
    [SerializeField] private Popup popup;
    [SerializeField] public Text hp, sp, day, time, money;
    private List<GameObject> etcList = new List<GameObject>();
    private PlayerData pd;
    private List<ConsEtc> etc = new List<ConsEtc>();

    public override void Awake()
    {
        for (int i = 0; i < etcPanel.transform.childCount; i++)
        {
            etcList.Add(etcPanel.transform.GetChild(i).gameObject);
        }
    }

    void Start()
    {
        pd = FosterManager.Instance.GetPlayerData();
    }

    private void Update()
    {
        if (GameManager.Instance.isPlay) SetUI();
    }

    private void SetEtcUI()
    {
        for(int i=0; i<etcList.Count; i++)
        {
            etcList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            etcList[i].SetActive(false);
        }
        for (int i = 0; i < pd.Cons.ETC.Count; i++)
        {
            etcList[i].SetActive(true);
            etcList[i].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("icons/" + pd.Cons.ETC[i].name, typeof(Sprite)) as Sprite;
            int imgIndex = i;
            int etcIndex = pd.Cons.ETC[i].index;
            etcList[i].GetComponent<Button>().onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowEtcDetailCoroutine(imgIndex, etcIndex)); });
        }
        etc = pd.Cons.ETC.ToList();
    }

    public void SetUI()
    {
        pd = FosterManager.Instance.GetPlayerData();
        hpGauge.fillAmount = pd.GetStatPercent(Const.hp);
        spGauge.fillAmount = pd.GetStatPercent(Const.sp);
        hp.text = pd.GetStateOutOfMax(Const.hp);
        sp.text = pd.GetStateOutOfMax(Const.sp);
        money.text = pd.GetMoney();
        day.text = GameManager.Instance.GetDDay();
        time.text = GameManager.Instance.GetCurrentTimeByValue();
        if (etc.Count==0 || etc.Intersect(pd.Cons.ETC).Count()!=etc.Count) SetEtcUI();
    }

    public void ActiveInteraction(Vector2 pos, string simulName)
    {
        interaction.SetActive(true);
        interaction.transform.position = pos + Vector2.up * 2;
        interaction.GetComponent<Image>().sprite = Resources.Load("Interact-" + simulName, typeof(Sprite)) as Sprite;
        if (simulName == "NPC")
        {
            interaction.GetComponent<Button>().onClick.AddListener(() => dialogBox.SetActive(true));
        }
        else interaction.GetComponent<Button>().onClick.AddListener(() => popup.DoAction(simulName));
    }

    public void HideInteraction()
    {
        interaction.GetComponent<Button>().onClick.RemoveAllListeners();
        interaction.SetActive(false);
    }

    private IEnumerator ShowEtcDetailCoroutine(int imgIndex, int etcIndex)
    {
        etcDetail.SetActive(true);
        etcDetail.GetComponent<Image>().rectTransform.anchoredPosition = etcPanel.transform.GetChild(imgIndex).GetComponent<Image>().rectTransform.anchoredPosition + new Vector2(150,-150);
        etcDetail.transform.GetChild(0).GetComponent<Text>().text = DataManager.Instance.GetEtcDetail(etcIndex);
        yield return new WaitForSeconds(2f);
        etcDetail.SetActive(false);
    }
}
