using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject sellItemGroup, invenItemGroup;
    [SerializeField] private Text moneyTxt;
    [SerializeField] private Button buyBtn;
    [SerializeField] private ItemInfo itemInfo;
    [SerializeField] private Image selectFrame;
    private GameObject[] items, invenItemEach;
    private Slot[] inventorySlots;
    private Button[] itemBtns;
    private Image[] itemBgs, itemImgs;
    private Text[] sellItemNames, sellItemPrices, invenItemEachNums;
    private Text selectedItemSkillTxt, buyBtnTxt;
    private int sellItemNum, itemNum, selectedItemIndex, selectedInvenListIndex;
    private PlayerData pd;
    private ItemData selectedItem;

    private ItemData[] itemDatas;

    private void Awake()
    {
        sellItemNum = sellItemGroup.transform.childCount;
        itemNum = sellItemNum + invenItemGroup.transform.childCount;

        itemBtns = new Button[itemNum];

        items = new GameObject[itemNum];
        invenItemEach = new GameObject[itemNum - sellItemNum];
        itemBgs = new Image[itemNum];
        itemImgs = new Image[itemNum];

        sellItemNames = new Text[sellItemNum];
        sellItemPrices = new Text[sellItemNum];
        invenItemEachNums = new Text[itemNum - sellItemNum];

        itemDatas = new ItemData[itemNum];

        for (int i=0; i< itemNum; i++)
        {

            // 가판대 아이템
            if (i < sellItemNum)
            {
                items[i] = sellItemGroup.transform.GetChild(i).gameObject;
                sellItemNames[i] = items[i].gameObject.transform.GetChild(1).GetComponent<Text>();
                sellItemPrices[i] = items[i].gameObject.transform.GetChild(2).GetComponent<Text>();
                itemBtns[i] = items[i].transform.GetChild(0).GetComponent<Button>();

                int index = i;
                itemBtns[i].onClick.AddListener(() => { Select(index); });
                itemBgs[i] = itemBtns[i].gameObject.GetComponent<Image>();
                itemImgs[i] = itemBtns[i].gameObject.transform.GetChild(0).GetComponent<Image>();
            }
            else
            {
                items[i] = invenItemGroup.transform.GetChild(i-sellItemNum).gameObject;
                itemImgs[i] = items[i].transform.GetComponent<Image>();
                //items[i] = invenItemGroup.transform.GetChild(i-sellItemNum).gameObject;
                //items[i].GetComponent<Slot>().isShopSlot(true);
                //itemBtns[i] = items[i].GetComponent<Button>();
                //invenItemEach[i-sellItemNum] = items[i].transform.GetChild(1).gameObject;
                //invenItemEachNums[i-sellItemNum] = invenItemEach[i-sellItemNum].transform.GetChild(0).GetComponent<Text>();
            }

        }

        // 인벤토리 슬롯 초기화
        inventorySlots = invenItemGroup.GetComponentsInChildren<Slot>();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].slotType = Const.inven;
            inventorySlots[i].slotIndex = i;
            inventorySlots[i].isShopSlot(true);
            inventorySlots[i].selectDel = Select;
        }

        buyBtnTxt = buyBtn.transform.GetChild(0).GetComponent<Text>();
    }


    private void OnEnable()
    {
        pd = FosterManager.Instance.GetPlayerData();
        itemInfo.gameObject.SetActive(false);
        selectFrame.enabled = false;
        SetSellItem();
        SetInvenItem();
        moneyTxt.text = pd.GetMoney();
    }

    // 가판대 아이템 세팅: 품절되었는지, 구매가능한지 체크 (가판대에 등장할 아이템 결정 함수는 별개로 구현 예정)
    // 팝업 열 때마다, 구입/판매 후 호출
    private void SetSellItem()
    {
        Dictionary<string, Equipment> equipDict = DataManager.Instance.GetAllEquipments();
        Dictionary<string, Potion> potionDict = DataManager.Instance.GetAllPotions();

        var equipList = new List<string>(equipDict.Keys);
        var potionList = new List<string>(potionDict.Keys);

        itemDatas[0] = equipDict[equipList[1]];
        itemDatas[1] = equipDict[equipList[2]];
        itemDatas[2] = potionDict[potionList[0]];

        for(int i=0; i<3; i++)
        {
            if (itemDatas[i].price > pd.Mon_Inven.Money)
            {
                if (i == selectedItemIndex) { 
                    itemInfo.gameObject.SetActive(false);
                    selectFrame.enabled=false;
                }
                itemBtns[i].interactable = false;
                itemImgs[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
            itemImgs[i].sprite = DataManager.Instance.GetSprite("Items", DataManager.Instance.GetKey(itemDatas[i]));
            sellItemNames[i].text = itemDatas[i].name;
            sellItemPrices[i].text = string.Format("{0:#,##0}", itemDatas[i].price.ToString());
        }
    }

    // 인벤토리 아이템 이미지 세팅
    // 팝업 열 때마다, 구입/판매 후 호출
    private void SetInvenItem()
    {
        //for(int i=sellItemNum; i<itemNum; i++)
        //{
        //    itemBtns[i].interactable = false;
        //    invenItemEach[i - sellItemNum].SetActive(false);

        //    Color color = itemImgs[i].color;
        //    color.a = 0f;
        //    itemImgs[i].color = color;
        //}

        for (int i = 0; i < itemNum-sellItemNum; i++)
        {
            inventorySlots[i].SetItem(null);
        }
        for (int i = 0; i < pd.Mon_Inven.Inven.Count; i++)
        {
            itemDatas[i + sellItemNum] = DataManager.Instance.GetItemData(pd.Mon_Inven.Inven[i]);
            inventorySlots[pd.Mon_Inven.Inven[i].inven_index].SetItem(pd.Mon_Inven.Inven[i]);
        }

        //for (int i = 0; i < pd.Mon_Inven.Inven.Count; i++)
        //{
        //    //item[pd.Mon_Inven.Inven[i].inven_index].SetItem(pd.Mon_Inven.Inven[i]);

        //    InvenItem invenItem = pd.Mon_Inven.Inven[i];
        //    int invenIndex = invenItem.inven_index + sellItemNum;

        //    itemBtns[invenIndex].interactable = true;
        //    itemImgs[invenIndex].sprite = DataManager.Instance.GetSprite("Items", invenItem.item_name);

        //    Color color = itemImgs[invenIndex].color;
        //    color.a = 1f;
        //    itemImgs[invenIndex].color = color;

        //    // 소비 아이템일 경우 개수 표시
        //    invenItemEach[invenIndex - sellItemNum].SetActive(DataManager.Instance.GetItemData(invenItem).category.Equals(Const.itemCategory[3]));
        //    invenItemEachNums[invenIndex - sellItemNum].text = invenItem.item_each.ToString();

        //    itemDatas[invenIndex] = DataManager.Instance.GetItemData(invenItem.item_name);
        //}
    }

    // 선택한 아이템 하이라이트, 아이템 정보 띄우기, 구매/판매 버튼 갱신
    private void Select(int index)
    {
        itemInfo.gameObject.SetActive(true);

        //if (selectedItemIndex != -1 && ) itemBgs[selectedItemIndex].color = new Color(1.0f, 1.0f, 1.0f);
        selectedItemIndex = index;
        //itemBgs[index].color = new Color(0.7f, 0.8f, 0.4f);
        // 테두리 이미지의 크기 및 위치를 현재 선택된 인덱스에 맞춤
        if (selectFrame.enabled == false) selectFrame.enabled=true;
        if (selectedItemIndex >= sellItemNum)
        {
            selectFrame.transform.position = items[selectedItemIndex].transform.position;
        }
        else selectFrame.transform.position = itemImgs[selectedItemIndex].transform.position;

        selectFrame.rectTransform.sizeDelta = itemImgs[selectedItemIndex].rectTransform.sizeDelta;


        // 가판대/인벤토리 아이템 선택 구분
        if (selectedItemIndex < sellItemNum)
        {
            //selectedItemInfoTxt.text = ((Equipment)DataManager.Instance.GetItemData(itemIds[selectedItemIndex])).GetItemDescription();
        }
        else
        {
            // 클릭한 인벤토리 아이템의 리스트 인덱스 찾기 (인벤토리 인덱스 X)
            for (int i = 0; i < pd.Mon_Inven.Inven.Count; i++)
            { 
                if(pd.Mon_Inven.Inven[i].inven_index == selectedItemIndex - sellItemNum)
                {
                    selectedInvenListIndex = i;
                    break;
                }
            }
        }


        itemInfo.SetItemInfo(itemDatas[selectedItemIndex]);

        if (!buyBtn.IsInteractable()) buyBtn.interactable = true;
        buyBtnTxt.text = ((selectedItemIndex < sellItemNum) ? "구매" : "판매");
    }

    public void BuyOrSell()
    {
        pd = FosterManager.Instance.GetPlayerData();

        // 가판대 아이템을 선택했을 경우 구매, 인벤토리 아이템을 선택했을 경우 판매
        if (selectedItemIndex < sellItemNum)
        {
            pd.Mon_Inven.Money -= itemDatas[selectedItemIndex].price;
            if (itemDatas[selectedItemIndex].category < 3) pd.AddInvenItem(itemDatas[selectedItemIndex]);  // equipment
            else pd.AddInvenItem(itemDatas[selectedItemIndex],-1, 1);
        }
        else
        {
            pd.Mon_Inven.Money += itemDatas[selectedItemIndex].price * 0.2f;

            // RemoveInvenItem 수정해야될듯.
            if (itemDatas[selectedItemIndex].category > 2)
            {
                pd.RemoveInvenItem(pd.Mon_Inven.Inven[selectedInvenListIndex], 1);
            }
            else pd.RemoveInvenItem(pd.Mon_Inven.Inven[selectedInvenListIndex]);

            itemInfo.gameObject.SetActive(false);
            selectFrame.enabled = false;
        }

        SetSellItem();
        SetInvenItem();
        moneyTxt.text = pd.GetMoney();
        UIManager_Area.Instance.SetUI();
    }
}
