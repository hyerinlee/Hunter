using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HunterAssociation : MonoBehaviour
{
    // 같은 구조의 모든 건물에 적용(헌터협회 only X)

    [SerializeField] private PlayerController playerController;

    private Collider2D buildingFloorCol, stairsCol;

    private bool isUp = true;  //false==down

    private void Awake()
    {

        buildingFloorCol = transform.GetChild(0).GetComponent<Collider2D>();
        stairsCol = transform.GetChild(1).GetComponent<Collider2D>();

        buildingFloorCol.enabled = false;
        stairsCol.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UseStairs();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        UseStairs();
    }

    public void UseStairs()
    {
        stairsCol.enabled = isUp;
        buildingFloorCol.enabled = !isUp;
        isUp = !isUp;
    }


}
