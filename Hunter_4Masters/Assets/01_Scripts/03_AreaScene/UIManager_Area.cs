using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager_Area : Singleton<UIManager_Area>
{
    [SerializeField] private Image hpGauge, spGauge;
    [SerializeField] private GameObject interaction, dialogBox;
    [SerializeField] private Popup popup;
    [SerializeField] public Text hp, sp, day, time, money;
    private PlayerData pd;
    private EventTrigger.Entry entry;

    public override void Awake()
    {
        
    }

    void Start()
    {
        pd = FosterManager.Instance.GetPlayerData();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
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
        interaction.GetComponent<SpriteRenderer>().sprite = Resources.Load("Interact-"+simulName, typeof(Sprite)) as Sprite;
        if (simulName == "NPC")
        {
            entry.callback.AddListener((eventData) => { dialogBox.SetActive(true); });
        }
        else entry.callback.AddListener((eventData) => { popup.DoAction(simulName); });
        interaction.GetComponent<EventTrigger>().triggers.Add(entry);
        Debug.Log(interaction.GetComponent<EventTrigger>().triggers.Count);
    }

    public void HideInteraction()
    {
        interaction.GetComponent<EventTrigger>().triggers.Clear();
        interaction.SetActive(false);
    }
}
