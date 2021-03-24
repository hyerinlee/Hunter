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
        sp = selectPanel.GetComponent<SelectionPopup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        sp.SetPopup(actionIndex);
        selectPanel.SetActive(true);
    }

}
