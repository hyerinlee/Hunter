using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private string interact;
    [SerializeField] private int spObjectCode = -1; // 병원 등 오브젝트 코드가 별도로 필요한 경우 지정

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIManager_Area.Instance.ActiveInteraction(this.transform.position, interact, spObjectCode);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UIManager_Area.Instance.HideInteraction();
    }
}
