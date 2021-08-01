using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager_Area : Singleton<UIManager_Area>
{
    [SerializeField] private Image hpGauge, spGauge;
    [SerializeField] private GameObject interaction, dialogBox, skillPotionPanel;
    [SerializeField] private GameObject[] potionEachs = new GameObject[2];
    [SerializeField] private EtcStates etcPanel;
    [SerializeField] private Popup popup;
    [SerializeField] public Text hp, sp, day, time, money;
    private GameObject[] skillPotionBtns = new GameObject[5];
    private Image[] skillPotionBtnImgs = new Image[5];
    private Text[] potioneachTxts = new Text[2];
    private Button interactBtn;
    private PlayerData pd;

    public override void Awake()
    {
        interactBtn = interaction.GetComponent<Button>();
        for(int i=0; i<Const.equipNum+Const.potionNum; i++)
        {
            skillPotionBtns[i] = skillPotionPanel.transform.GetChild(i).gameObject;
            skillPotionBtnImgs[i] = skillPotionBtns[i].transform.GetChild(0).GetComponent<Image>();
            if (i >= Const.equipNum)
            {
                potionEachs[i-Const.equipNum] = skillPotionBtns[i].transform.GetChild(1).gameObject;
                potioneachTxts[i - Const.equipNum] = potionEachs[i - Const.equipNum].transform.GetChild(0).GetComponent<Text>();
            }
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
        for (int i = 0; i < potionEachs.Length; i++) potionEachs[i].SetActive(false);
        for(int i=0; i<skillPotionBtns.Length; i++)
        {
            if (pd.Mon_Inven.Equipment[i].item_index != 0)
            {
                // set sprite
                skillPotionBtnImgs[i].sprite = DataManager.Instance.GetSprite("Items", pd.Mon_Inven.Equipment[i].item_name);
                skillPotionBtnImgs[i].enabled = true;

                // onclick addlistener
                int index = i;
                skillPotionBtns[i].GetComponent<Button>().onClick.RemoveAllListeners();

                if (i < Const.equipNum)
                {
                    // skill
                    skillPotionBtns[i].GetComponent<Button>().onClick.AddListener(() => { Debug.Log("스킬 사용함:" + pd.Mon_Inven.Equipment[index].item_name); });
                }
                else
                {
                    potionEachs[i - Const.equipNum].SetActive(true);
                    potioneachTxts[i - Const.equipNum].text = FosterManager.Instance.GetPlayerData().Mon_Inven.Equipment[index].item_each.ToString();

                    // potion
                    skillPotionBtns[i].GetComponent<Button>().onClick.AddListener(() => 
                    {
                        Debug.Log("포션 1개 사용함");
                        FosterManager.Instance.GetPlayerData().UsePotion(FosterManager.Instance.GetPlayerData().Mon_Inven.Equipment[Const.equipNum+i]);
                        SetSkills();
                    });
                }
            }
            else
            {
                skillPotionBtnImgs[i].enabled = false;
                skillPotionBtns[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    public void ActiveInteraction(Vector2 pos, string simulName)
    {
        interaction.transform.position = pos + Vector2.up * 2;
        interaction.GetComponent<Image>().sprite = DataManager.Instance.GetSprite("", "Interact-" + simulName);
        interactBtn.onClick.RemoveAllListeners();
        if (simulName == "NPC")
        {
            interactBtn.onClick.AddListener(() => dialogBox.SetActive(true));
        }
        else interactBtn.onClick.AddListener(() => popup.DoAction(simulName));
        interaction.SetActive(true);
    }

    public void HideInteraction()
    {
        interactBtn.onClick.RemoveAllListeners();
        interaction.SetActive(false);
    }

}
