using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;



public class PlayerInfoPopup : Singleton<PlayerInfoPopup>
{
    [SerializeField] private Text battleStats, money;
    [SerializeField] private GameObject equipment, inventory, defaultStats, states;
    private Slot[] equipmentSlots, inventorySlots;
    private Text[] defStatsTxt = new Text[3];
    private Text[] statesTxt = new Text[3];
    private PlayerData pd;

    public override void Awake()
    {
        for(int i=0; i<3; i++)
        {
            defStatsTxt[i] = defaultStats.transform.GetChild(i).GetComponent<Text>();
            statesTxt[i] = states.transform.GetChild(i).GetComponent<Text>();
        }
        equipmentSlots = equipment.GetComponentsInChildren<Slot>();
        for(int i=0; i<equipmentSlots.Length; i++)
        {
            equipmentSlots[i].slotType = Const.equip;
            equipmentSlots[i].slotIndex = i;
            equipmentSlots[i].defaultSprite = equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite;
        }
        inventorySlots = inventory.GetComponentsInChildren<Slot>();
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].slotType = Const.inven;
            inventorySlots[i].slotIndex = i;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.Pause();
        setInfo();
    }

    private void OnDisable()
    {
        GameManager.Instance.Resume();
    }

    public void setInfo()
    {
        pd = FosterManager.Instance.GetPlayerData();

        for(int i=0; i<3; i++)
        {
            defStatsTxt[i].text = pd.GetDefStat(pd.Stats.Keys.ElementAt(i));
            statesTxt[i].text = pd.Cons.n_Cons.Values.ElementAt(i).name +" " + pd.GetStateOutOfMax(pd.Cons.n_Cons.Keys.ElementAt(i));
        }

        battleStats.text = pd.GetBatStat();
        money.text = pd.GetMoney();

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].SetItem(pd.Mon_Inven.Equipment[i]);
        }
        for (int i=0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetItem(null);
        }
        for(int i=0; i < pd.Mon_Inven.Inven.Count; i++)
        {
            inventorySlots[pd.Mon_Inven.Inven[i].inven_index].SetItem(pd.Mon_Inven.Inven[i]);
        }
    }
}
