using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 터치 처리를 위한 인터페이스 상속
public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image joystickArea;
    private Image joystick;
    private Vector2 inputVector;    //이동 벡터값

    void Start()
    {
        joystickArea = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
    }

    // IDragHandler 인터페이스에 선언된 OnDrag 메소드 구현
    // 터치중일 동안 조이스틱 이동 및 이동벡터값 저장
    public void OnDrag(PointerEventData ped)
    {   
        Vector2 pos;
        // 터치 시작 위치가 조이스틱 배경 이내이면 pos에 로컬 좌표를 저장
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickArea.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            // pixel 단위로 되어있는 pos 값을 조이스틱 배경크기에 대한 비율(-1 ~ 1)로 변환
            pos.x = (pos.x / joystickArea.rectTransform.sizeDelta.x)*2;
            pos.y = (pos.y / joystickArea.rectTransform.sizeDelta.y)*2;

            inputVector = pos;
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // 조이스틱을 앵커(=조이스틱 배경의 중심)를 기준으로 터치한 곳으로 이동
            joystick.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystickArea.rectTransform.sizeDelta.x * 0.4f),
                inputVector.y * (joystickArea.rectTransform.sizeDelta.y * 0.4f));
        }
    }

    // IPointerUpHandler 인터페이스에 선언된 OnPointerDown 메소드 구현
    // 터치 시작 시 호출
    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    // IPointerDownHandler 인터페이스에 선언된 OnPointerUp 메소드 구현
    // 뗐을 때 조이스틱 위치 리셋
    public void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector2.zero;
        joystick.rectTransform.anchoredPosition = Vector2.zero;
    }

    // x값 반환(플레이어 스크립트에서 사용)
    public float GetHorizontalValue()
    {
        return inputVector.x;
    }

    // y값 반환(이 게임에서는 사용하지 않음)
    //public float GetVerticalValue()
    //{
    //    return inputVector.y;
    //}
}
