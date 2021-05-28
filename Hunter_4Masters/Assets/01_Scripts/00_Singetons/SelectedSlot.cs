using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedSlot : Singleton<SelectedSlot>
{
    public RectTransform rt;    // 터치 좌표와 오브젝트 좌표 맞춤 기준

    [SerializeField] private GameObject itemInfo; // 아이템 정보창
    [SerializeField] private Image dragImage;     // 드래그 시 활성화될 이미지
    [SerializeField] private GameObject equipErrorMsg;

    public PlayerItem playerItem;    // 드래그 or 정보확인 대상 아이템
    private Text itemNameTxt, itemInfoTxt;

    public Vector2 dragOffset = new Vector2(0, 0);
    public Vector2 InfoOffset = new Vector2(-200,200);
    private Vector2 inputDir;   // 터치 좌표

    private void Start()
    {
        itemNameTxt = itemInfo.transform.GetChild(0).GetComponent<Text>();
        itemInfoTxt = itemInfo.transform.GetChild(1).GetComponent<Text>();
    }

    public void SetSelectedItem(PlayerItem playerItem)
    {
        this.playerItem = playerItem;
    }

    public void SetDragSlotPos(PointerEventData ped)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, ped.position, Camera.main, out inputDir);
        dragImage.rectTransform.anchoredPosition = inputDir + dragOffset;
    }
    public void SetItemInfoPos(PointerEventData ped)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, ped.position, Camera.main, out inputDir);
        itemInfo.GetComponent<Image>().rectTransform.anchoredPosition = inputDir+InfoOffset;
    }

    public void SetActiveItemInfo(bool activeState)
    {
        itemInfo.SetActive(activeState);
        if(activeState && playerItem != null)
        {
            itemNameTxt.text = DataManager.Instance.GetItemData(playerItem.item_name).name;
            itemInfoTxt.text = DataManager.Instance.GetItemData(playerItem.item_name).GetItemDescription();
        }
    }

    public void ResetDragSlot()
    {
        SetDragImageColor(0);
        playerItem = null;
        dragImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetDragSlotImage(Image itemImage)
    {
        dragImage.sprite = itemImage.sprite;
        SetDragImageColor(0.5f);
    }

    public void SetDragImageColor(float alpha)
    {
        Color color = dragImage.color;
        color.a = alpha;
        dragImage.color = color;
    }
}
