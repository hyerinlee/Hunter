using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_Area : Singleton<UIManager_Area>
{
    [SerializeField] private Image hpGauge, spGauge;
    [SerializeField] private GameObject interaction, dialogBox;
    [SerializeField] private Popup popup;
    [SerializeField] public Text hp, sp, day, time, money;
    private PlayerData pd;

    public override void Awake()
    {
        
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
        hpGauge.fillAmount = pd.Cons["HP"].cur_point / pd.Cons["HP"].max_point;
        spGauge.fillAmount = pd.Cons["SP"].cur_point / pd.Cons["SP"].max_point;
        hp.text = pd.GetStateOutOfMax("HP");
        sp.text = pd.GetStateOutOfMax("SP");
        money.text = pd.GetMoney();
        day.text = GameManager.Instance.GetDDay();
        time.text = GameManager.Instance.GetCurrentTimeByValue();
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
}
