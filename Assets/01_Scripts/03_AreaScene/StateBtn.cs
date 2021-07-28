using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StateBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject interact;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("버튼누름");
        interact.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("버튼 뗌");
        interact.SetActive(false);
    }
}
