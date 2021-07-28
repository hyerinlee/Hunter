using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField] private GameObject selectPanel, shopPanel;
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

        if (action == "shop")
        {
            shopPanel.SetActive(true);
        }
        else
        {
            selectPanel.SetActive(true);
            selectionPopup.SetPopup(action);
        }
    }
}
