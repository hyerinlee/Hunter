using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaNPC : MonoBehaviour
{
    [SerializeField] private GameObject interact;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interact.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interact.SetActive(false);
    }
}
