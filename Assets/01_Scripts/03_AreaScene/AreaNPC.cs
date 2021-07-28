using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaNPC : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIManager_Area.Instance.ActiveInteraction(this.transform.position, "NPC");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UIManager_Area.Instance.HideInteraction();
    }
}
