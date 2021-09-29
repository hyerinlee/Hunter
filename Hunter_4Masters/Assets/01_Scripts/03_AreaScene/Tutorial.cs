using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public InputField InputTxt;
    public GameObject NameBox;

    protected GameObject canvas;
    protected GameObject DialogBox;
    protected Talk talkScript;

    private bool getName = false;

    private bool triggerOn = false;

    [SerializeField] private GameObject leaflet;
    [SerializeField] private GameObject skipBtn;
    [SerializeField] private GameObject exitBtn;

    public string name = "";

    public void StartTutorial()
    {
        talkScript.txt = Resources.Load<TextAsset>($"Scripts/TutorialScript");

        DialogBox.SetActive(true);
       // DialogBox.GetComponent<Talk>().StartDialog();

    }

    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        DialogBox = canvas.transform.Find("DialogBox").gameObject;
        talkScript = DialogBox.GetComponent<Talk>();
        
        // //Test
        //DialogBox.TestStart();
    }

    void Update()
    {
        switch(talkScript.currentIdx){
            case 3:
                if(!triggerOn)
                {
                    triggerOn = true;
                    leaflet.SetActive(true);
                }
                break;
            case 10:
                triggerOn = false;
                leaflet.SetActive(false);
                if(!getName) GetName();
                break;
        }

    }

    void GetName()
    {
        getName = true;

        skipBtn.SetActive(false);
        exitBtn.SetActive(false);

        if(NameBox.activeSelf != true)
        {
            NameBox.SetActive(true);
            //test
            //DialogBox.SetActive(false);
        }
    }

    public void SetName()
    {
        try{
            name = InputTxt.text;

            if(isCorrectName(name))
            {
                Debug.Log("내 이름은 " + name);
                InputTxt.text = "";
                NameBox.SetActive(false);
                //test
                //DialogBox.SetActive(true);
                //DialogBox.TestStart();
                //talkScript.currentIdx = 11;
                skipBtn.SetActive(true);
                exitBtn.SetActive(true);
            }
            else
            {
                Debug.Log("이상한 이름임. 다시!");
                InputTxt.text = "";
                name = "";
                GetName();
            }
        }catch(ArgumentOutOfRangeException e){
            Debug.Log("ArgumentOutOfRangeException 발생!!");
            GetName();
        }
        
        // Talk 스크립트의 currentIdx++ , Play 함수 실행 
    }

    bool isCorrectName(string name)
    {
        // 한글 혹은 영어여야 함
        // 특수문자는 허용되지 않음
        // 최소 2글자, 최대 8글자 (한글인 경우이고 영어와 숫자인 경우도 설정해야함)
        
        Regex regex = new Regex(@"^[0-9a-zA-Zㄱ-ㅎㅏ-ㅣ가-힣]{2,8}$");
        if(!(regex.IsMatch(name)))
        {
            Debug.Log("잘못입력함 : " + name);
            return false;
        }
        else
        {
            Debug.Log("제대로 입력함 : " + name);
            return true;
        }
    }
}