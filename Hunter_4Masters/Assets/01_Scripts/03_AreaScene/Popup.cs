using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel;
    private SelectionPopup selectionPopup;


    void Start()
    {
        selectionPopup = selectPanel.GetComponent<SelectionPopup>();
    }

    public SelectionPopup GetSelectionPopup()
    {
        return selectionPopup;
    }

    public void DoAction(string action)
    {
        // 시간 일시정지 구현예정

        selectPanel.SetActive(true);
        selectionPopup.SetPopup(action);
    }
}
