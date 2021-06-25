using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EtcStates : MonoBehaviour
{
    private PlayerData pd;
    private GameObject[] etcs = new GameObject[3];
    private Image[] etcImgs = new Image[3];
    private Button[] etcBtns = new Button[3];
    private GameObject etcDetail;

    private List<ConsEtc> etc = new List<ConsEtc>();

    private void Awake()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            etcs[i] = transform.GetChild(0).GetChild(i).gameObject;
            etcImgs[i] = etcs[i].transform.GetChild(0).GetComponent<Image>();
            etcBtns[i] = etcs[i].GetComponent<Button>();
        }
        etcDetail = transform.GetChild(1).gameObject;
    }

    public void SetEtcUI()
    {
        // 기타 상태가 바뀌지 않았으면 리턴
        if (etc.Count != 0 && etc.Intersect(pd.Cons.ETC).Count() == etc.Count) return;

        pd = FosterManager.Instance.GetPlayerData();
        for (int i = 0; i < etcs.Length; i++)
        {
            etcBtns[i].onClick.RemoveAllListeners();
            etcs[i].SetActive(false);
        }
        for (int i = 0; i < pd.Cons.ETC.Count; i++)
        {
            etcs[i].SetActive(true);
            etcImgs[i].sprite = Resources.Load("icons/" + pd.Cons.ETC[i].name, typeof(Sprite)) as Sprite;
            int imgIndex = i;
            int etcIndex = pd.Cons.ETC[i].index;
            etcBtns[i].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowEtcDetailCoroutine(imgIndex, etcIndex)); });
        }

        etc = pd.Cons.ETC.ToList();
    }

    private IEnumerator ShowEtcDetailCoroutine(int imgIndex, int etcIndex)
    {
        etcDetail.SetActive(true);
        etcDetail.GetComponent<Image>().rectTransform.anchoredPosition = etcs[imgIndex].GetComponent<Image>().rectTransform.anchoredPosition + new Vector2(50,-50);
        etcDetail.transform.GetChild(0).GetComponent<Text>().text = DataManager.Instance.GetEtcDetail(etcIndex);
        yield return new WaitForSeconds(2f);
        etcDetail.SetActive(false);
    }
}
