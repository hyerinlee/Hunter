using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Labor : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UIManager_Area.Instance.ActiveInteraction(this.transform.position, "labor");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UIManager_Area.Instance.HideInteraction();
    }
}
