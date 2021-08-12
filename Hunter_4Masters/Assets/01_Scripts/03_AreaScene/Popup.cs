using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel, shopPanel;
    private SelectionPopup selectionPopup;
    private RectTransform selectionPopupRT;

    private float slideMidPos = -840f;  // 선택지 팝업창 슬라이드 전·후의 중간지점
    private float slideDelta = 340f; // 선택지 팝업창 슬라이드 이동량

    void Start()
    {
        selectionPopup = selectPanel.GetComponent<SelectionPopup>();
        selectionPopupRT = selectPanel.transform.GetChild(0).GetComponent<RectTransform>();
    }

    public SelectionPopup GetSelectionPopup()
    {
        return selectionPopup;
    }

    public void DoAction(string action)
    {
        GameManager.Instance.Pause();

        if (action == "shop")
        {
            shopPanel.SetActive(true);
        }
        else
        {
            StopAllCoroutines();

            GameManager.Instance.SetController(false);
            GameManager.Instance.Zoom(Vector3.forward);
            selectPanel.SetActive(true);
            StartCoroutine(PopupSlideCoroutine(Vector3.left));

            selectionPopup.SetPopup(action);
        }
    }

    // 선택지 팝업창 닫기 버튼에서 호출
    public void CancelAction()
    {
        StopAllCoroutines();

        GameManager.Instance.Zoom(Vector3.back);
        StartCoroutine(popupExitCoroutine());
    }

    // 선택지 팝업창 닫을 때: 슬라이드 후 비활성화 해야 함
    private IEnumerator popupExitCoroutine()
    {
        StartCoroutine(PopupSlideCoroutine(Vector3.right));
        yield return new WaitForSeconds(0.5f);
        selectPanel.SetActive(false);
        GameManager.Instance.SetController(true);
        GameManager.Instance.Resume();
        yield return null;
    }

    // 선택지 팝업창 슬라이드 코루틴
    private IEnumerator PopupSlideCoroutine(Vector3 dir)
    {
        Vector2 destPos = new Vector2(slideMidPos + slideDelta * dir.x, selectionPopupRT.anchoredPosition.y);
        float offset = 0f;
        while (selectionPopupRT.anchoredPosition != destPos && offset < 1.5f)
        {
            selectionPopupRT.anchoredPosition = Vector2.Lerp(selectionPopupRT.anchoredPosition, destPos, Time.deltaTime*4.5f);
            offset += Time.deltaTime;
            yield return null;
        }
        selectionPopupRT.anchoredPosition = destPos;
    }
}
