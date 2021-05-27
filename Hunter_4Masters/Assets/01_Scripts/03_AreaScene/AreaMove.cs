using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMove : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    private Popup popupScript;

    private void Start()
    {
        popupScript = popup.GetComponent<Popup>();
        // 콜라이더 좌표를 해상도에 맞게 지정(오른쪽 가장자리)
        this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.03f, 0.6f, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        popupScript.DoAction("Move");
    }

}