using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("구입했습니다");
        Buy();
    }

    public void Buy()
    {
        PlayerData pd = FosterManager.Instance.GetPlayerData();
        pd.Mon_Inven.Money -= 3;

        pd.AddInvenItem("axe", 1);
        pd.AddInvenItem("potionhp", 3);
        pd.AddInvenItem("strongArmor", 1);
        pd.AddInvenItem("cape", 1);
        pd.AddInvenItem("blueshoes", 1);
    }
}
