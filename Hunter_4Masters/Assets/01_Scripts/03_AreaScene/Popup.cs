using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameManager.Instance.Pause();

        selectPanel.SetActive(true);
        selectionPopup.SetPopup(action);
    }
}
