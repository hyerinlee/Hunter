using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EtcStates : MonoBehaviour
{
    [SerializeField] private bool isMainUI = false;

    private PlayerData pd;
    private GameObject[] etcs = new GameObject[Const.etcMaxSize];
    private Image[] etcImgs = new Image[Const.etcMaxSize];
    private Button[] etcBtns = new Button[Const.etcMaxSize];
    private GameObject etcDetail;   // 거주지(메인)에서는 스크롤뷰, 정보창에서는 작은 팝업창에 해당
    private EtcDetailList etcDetailList = null; // 거주지(메인)에서만 사용

    private List<ConsEtc> etc = new List<ConsEtc>();

    private void Awake()
    {
        for (int i = 0; i < Const.etcMaxSize; i++)
        {
            etcs[i] = transform.GetChild(0).GetChild(i).gameObject;
            etcImgs[i] = etcs[i].transform.GetChild(0).GetComponent<Image>();
            etcBtns[i] = etcs[i].GetComponent<Button>();
        }
        etcDetail = transform.GetChild(1).gameObject;
        etcDetail.SetActive(false);

        if (isMainUI) etcDetailList = etcDetail.transform.GetChild(0).GetComponent<EtcDetailList>();
        
        SetEtcUI();
    }

    // 정보창에서의 기타상태 세팅
    public void SetEtcUI()
    {
        // 기타 상태가 바뀌지 않았으면 리턴
        if (etc.Count != 0 && etc.Intersect(pd.Cons.ETC).Count() == etc.Count) return;

        pd = FosterManager.Instance.GetPlayerData();

        // 변경 전의 기타상태 UI 리셋
        for (int i = 0; i < Const.etcMaxSize; i++)
        {
            if (!isMainUI) etcBtns[i].onClick.RemoveAllListeners();
            etcs[i].SetActive(false);
        }

        for (int i = 0; i < pd.Cons.ETC.Count; i++)
        {
            etcs[i].SetActive(true);
            etcImgs[i].sprite = DataManager.Instance.GetSprite("icons", pd.Cons.ETC[i].name);
            if (!isMainUI)
            {
                int imgIndex = i;
                int etcIndex = pd.Cons.ETC[i].index;
                etcBtns[i].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(EtcDetailCo(imgIndex, etcIndex)); });
            }
        }

        if(isMainUI) etcDetailList.SetScrollView(pd);

        etc = pd.Cons.ETC.ToList();
    }


    private IEnumerator EtcDetailCo(int imgIndex, int etcIndex)
    {
        etcDetail.SetActive(true);
        etcDetail.GetComponent<Image>().rectTransform.anchoredPosition = etcs[imgIndex].GetComponent<Image>().rectTransform.anchoredPosition + new Vector2(50,-50);
        etcDetail.transform.GetChild(0).GetComponent<Text>().text = DataManager.Instance.GetEtcDetail(etcIndex);
        yield return new WaitForSeconds(2f);
        etcDetail.SetActive(false);
    }
}
