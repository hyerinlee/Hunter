using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EtcDetailList : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    private GameObject parent;
    private RectTransform scrollContent;

    private GameObject[] mainEtcDetails = new GameObject[Const.etcMaxSize];
    private Image[] mainEtcDetailImgs = new Image[Const.etcMaxSize];
    private Text[] mainEtcDetailTxts = new Text[Const.etcMaxSize];

    private bool isTouching = false;

    private void Awake()
    {
        for (int i = 0; i < Const.etcMaxSize; i++)
        {
            //               ScrollView -> Viewport -> Content -> EtcDetail[i]
            mainEtcDetails[i] = transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
            //                                   EtcDetail[i] -> EtcBg -> Img_Etc
            mainEtcDetailImgs[i] = mainEtcDetails[i].transform.GetChild(0).GetChild(0).GetComponent<Image>();
            //                                   EtcDetail[i] -> Txt_EtcDesc
            mainEtcDetailTxts[i] = mainEtcDetails[i].transform.GetChild(1).GetComponent<Text>();
        }

        parent = this.transform.parent.gameObject;
        scrollContent = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
    }

    // 거주지(메인)화면에서 기타상태 아이콘 리스트 세팅할 때 같이 세팅됨
    public void SetScrollView(PlayerData pd)
    {
        for (int i = 0; i < Const.etcMaxSize; i++)
        {
            mainEtcDetails[i].SetActive(false);
        }
        for (int i = 0; i < pd.Cons.ETC.Count; i++)
        {
            mainEtcDetails[i].SetActive(true);
            mainEtcDetailImgs[i].sprite = DataManager.Instance.GetSprite("icons", pd.Cons.ETC[i].name);
            int etcIndex = pd.Cons.ETC[i].index;
            mainEtcDetailTxts[i].text = DataManager.Instance.GetEtcDetail(etcIndex);
        }
    }

    public void ShowScrollView()
    {
        parent.SetActive(true);

        // 스크롤 위치 리셋
        float x = scrollContent.anchoredPosition.x;
        scrollContent.anchoredPosition = new Vector3(x, 0, 0);

        StartCoroutine(MainEtcDetailCrt());
    }

    private IEnumerator MainEtcDetailCrt()
    {
        parent.SetActive(true);
        float waitTime = 0f;
        while (waitTime < 2.0f)
        {
            waitTime = (isTouching ? 0f : waitTime + Time.deltaTime);
            yield return null;
        }
        parent.SetActive(false);
    }
}
