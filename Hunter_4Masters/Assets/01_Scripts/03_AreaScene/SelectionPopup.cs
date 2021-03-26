using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionPopup : MonoBehaviour
{
    [SerializeField] private Image actionImg;
    [SerializeField] private Text actionInfo;
    [SerializeField] private Text[] categories = new Text[5];
    [SerializeField] private GameObject[] options = new GameObject[3];


    public Sprite[] actionImgTempData = new Sprite[2];
    private string[] actionInfoTempData = { "평화로운 산골마을에 위치한 지리산은 가는 길은 그다지 평화롭지 않을 수도 있습니다.",
                                        "헌터협회 사무보조 인력 모집 중\n\n" +
                                        "근무시간: 시간협의\n" +
                                        "제출서류: 이력서" };
    private string[,] categoriesTempData = { { "종류", "비용", "시간", "스태미나", "위험도" },
                                        { "종류", "일당", "시간", "스태미나", "위험도" } };
    private string[,,] optionsTempData = { { {"도보","0$","5시간","30","위험" },
                                        {"대중교통","50$","3시간","10","보통" },
                                        {"호위이동","500$","4시간","10","안전" }},

                                      { {"일용직 알바","500$","3시간","100","위험" },
                                        {"짐꾼","100$","5시간","50","보통" },
                                        {"헌터협회","50$","1시간","10","안전" }} };


    public void SetPopup(int actionIndex)
    {

        // 임시 적용 코드
        actionImg.sprite = actionImgTempData[actionIndex];
        actionInfo.text = actionInfoTempData[actionIndex];
        for(int i = 0; i < 5; i++)
        {
            categories[i].text = categoriesTempData[actionIndex,i];
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                options[i].transform.GetChild(j).GetComponent<Text>().text = optionsTempData[actionIndex, i, j];
                options[i].transform.GetComponent<ActionOption>().SetIndex(actionIndex, i);
            }
        }

    }
}
