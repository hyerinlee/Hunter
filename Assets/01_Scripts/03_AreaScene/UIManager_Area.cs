using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager_Area : Singleton<UIManager_Area>
{
    [SerializeField] private Image hpGauge, spGauge;
    [SerializeField] private GameObject interaction, dialogBox, skillPanel;
    [SerializeField] private EtcStates etcPanel;
    [SerializeField] private Popup popup;
    [SerializeField] public Text hp, sp, day, time, money;
    private List<GameObject> skillBtnList = new List<GameObject>();
    private PlayerData pd;

    public override void Awake()
    {
        for (int i = 0; i < skillPanel.transform.childCount - 2; i++)
        {
            skillBtnList.Add(skillPanel.transform.GetChild(i).gameObject);
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
        etcPanel.SetEtcUI();
    }

    public void SetSkills()
    {
        for(int i=0; i<skillBtnList.Count; i++)
        {
            Image skillImg = skillBtnList[i].transform.GetChild(0).GetComponent<Image>();
            if (pd.Mon_Inven.Equipment[i].item_index != 0)
            {
                // set skill sprite
                skillImg.sprite = Resources.Load("Items/" + pd.Mon_Inven.Equipment[i].item_name, typeof(Sprite)) as Sprite;
                skillImg.enabled = true;
                // skill onclick addlistener
                int skillIndex = i;
                skillBtnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
                skillBtnList[i].GetComponent<Button>().onClick.AddListener(() => { Debug.Log("스킬 사용함:" + pd.Mon_Inven.Equipment[skillIndex].item_name); });
            }
            else
            {
                skillImg.enabled = false;
                skillBtnList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    public void ActiveInteraction(Vector2 pos, string simulName)
    {
        interaction.SetActive(true);
        interaction.transform.position = pos + Vector2.up * 2;
        interaction.GetComponent<Image>().sprite = Resources.Load("Interact-" + simulName, typeof(Sprite)) as Sprite;
        interaction.GetComponent<Button>().onClick.RemoveAllListeners();
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

}
