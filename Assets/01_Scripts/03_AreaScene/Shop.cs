using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopItemGroup;
    [SerializeField] private Text shopItemInfoTxt, money;
    [SerializeField] private Button buyButton;
    private Button[] shopItems;
    private Image[] shopItemBgs, shopItemImgs;
    private Text[] shopItemNames, shopItemPrices;
    private int selectedShopItemIndex;

    private void Awake()
    {
        shopItems = shopItemGroup.GetComponentsInChildren<Button>();
        shopItemBgs = new Image[shopItems.Length];
        shopItemImgs = new Image[shopItems.Length];
        shopItemNames = new Text[shopItems.Length];
        shopItemPrices = new Text[shopItems.Length];
        for (int i=0; i<shopItemGroup.transform.childCount; i++)
        {
            int index = i;
            shopItems[i].onClick.AddListener(()=> { Select(index); });
            shopItemBgs[i] = shopItems[i].gameObject.GetComponent<Image>();
            shopItemImgs[i] = shopItems[i].gameObject.transform.GetChild(0).GetComponent<Image>();
            shopItemNames[i] = shopItems[i].gameObject.transform.GetChild(1).GetComponent<Text>();
            shopItemPrices[i] =  shopItems[i].gameObject.transform.GetChild(2).GetComponent<Text>();
        }
    }


    private void OnEnable()
    {
        ResetSelect();
        money.text = FosterManager.Instance.GetPlayerData().GetMoney();
        Dictionary<string, Equipment> items = DataManager.Instance.GetAllEquipments();
        int index = 0;
        foreach(KeyValuePair<string,Equipment> item in items)
        {
            shopItemImgs[index].sprite = Resources.Load("Items/" + item.Key, typeof(Sprite)) as Sprite;
            shopItemNames[index].text = item.Key;
            shopItemPrices[index].text = string.Format("{0:#,##0}", item.Value.price.ToString()) + "$";
            ++index;
        }

    }

    private void Select(int index)
    {
        if(selectedShopItemIndex != -1) shopItemBgs[selectedShopItemIndex].color = new Color(1.0f, 1.0f, 1.0f);
        selectedShopItemIndex = index;
        shopItemInfoTxt.text = DataManager.Instance.GetItemData(shopItemNames[selectedShopItemIndex].text).GetItemDescription();
        shopItemBgs[index].color = new Color(0.7f, 0.8f, 0.4f);
        if (!buyButton.IsInteractable()) buyButton.interactable = true;
    }

    private void ResetSelect()
    {
        if(selectedShopItemIndex != -1) shopItemBgs[selectedShopItemIndex].color = new Color(1.0f, 1.0f, 1.0f);
        selectedShopItemIndex = -1;
        shopItemInfoTxt.text = "";
        buyButton.interactable = false;
    }

    public void Buy()
    {
        PlayerData pd = FosterManager.Instance.GetPlayerData();
        ItemData id = DataManager.Instance.GetItemData(shopItemNames[selectedShopItemIndex].text);
        pd.Mon_Inven.Money -= id.price;
        pd.AddInvenItem(shopItemNames[selectedShopItemIndex].text);  // equipment

        money.text = pd.GetMoney();
        for(int i=0; i<shopItems.Length; i++)
        {
            if(DataManager.Instance.GetItemData(shopItemNames[i].text).price > pd.Mon_Inven.Money)
            {
                if (i == selectedShopItemIndex) ResetSelect();
                shopItems[i].interactable = false;
                shopItemImgs[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        UIManager_Area.Instance.SetUI();
    }
}
