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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        popupScript.DoAction("Move");
    }

}