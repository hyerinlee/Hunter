using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private string interact;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIManager_Area.Instance.ActiveInteraction(this.transform.position, interact);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UIManager_Area.Instance.HideInteraction();
    }
}
