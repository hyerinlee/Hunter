using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaMove : MonoBehaviour
{
    [SerializeField]
    private GameObject selectPanel;

    private SelectionPopup sp;
    private int actionIndex = 0;    // 임시로 이동index == 0 으로 지정

    private void Start()
    {
        // 콜라이더 좌표를 해상도에 맞게 지정(오른쪽 가장자리)
        this.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.03f, 0.6f, 0));
        sp = selectPanel.GetComponent<SelectionPopup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        sp.SetPopup(actionIndex);
        selectPanel.SetActive(true);
    }

}