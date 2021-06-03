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

        pd.AddInvenItem("001_Axe", 1);
        pd.AddInvenItem("00_HP_Potion_S", 3);
        pd.AddInvenItem("004_StrongArmor", 1);
        pd.AddInvenItem("002_Cape", 1);
        pd.AddInvenItem("003_BlueShoes", 1);
    }
}
