using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private GameObject skill;
    [SerializeField] private Image itemImg, rankImg, skillImg;
    [SerializeField] private Text itemNameTxt, itemDescTxt, skillTxt;

    public void SetItemInfo(ItemData id)
    {
        itemImg.sprite = DataManager.Instance.GetSprite("Items", DataManager.Instance.GetKey(id));
        rankImg.sprite = DataManager.Instance.GetSprite("icons", "rank_" + id.rank);

        itemNameTxt.text = id.name + "\n" + Const.itemCategory[id.category];
        itemDescTxt.text = "";

        if (id.GetType() == typeof(Equipment))
        {
            for (int i = 0; i < ((Equipment)id).condition.Count; i++)
            {
                if (i == 0) itemDescTxt.text = "장착조건\n";
                itemDescTxt.text += GetConditionRange(((Equipment)id).condition[i]) + "\n";
            }

            for (int i = 0; i < id.effect.Count; i++)
            {
                if (i == 0) itemDescTxt.text += "기본 효과\n";
                itemDescTxt.text += id.effect[i].effect_variable +
                                    GetEffectRange(id.effect[i].effect_min, id.effect[i].effect_max) + "\n";
            }

            skill.SetActive(true);
            skillImg.sprite = DataManager.Instance.GetSprite("icons", ((Equipment)id).skillIndex.ToString());
            skillTxt.text = ((Equipment)id).skillIndex.ToString();


        }
        else
        {
            itemDescTxt.text = "";

            for (int i = 0; i < id.effect.Count; i++)
            {
                itemDescTxt.text += id.effect[i].effect_variable + " " + 
                                    GetEffectRange(id.effect[i].effect_min, id.effect[i].effect_max) + " 회복\n";
            }

            skill.SetActive(false);
        }

    }

    private string GetConditionRange(Condition con)
    {
        bool hasMinLimit = Mathf.Abs(con.condition_min - Const.minDef) > 0.0001f;
        bool hasMaxLimit = Mathf.Abs(con.condition_max - Const.maxDef) > 0.0001f;
        string str="";

        if (hasMinLimit || hasMaxLimit)
        {
            str += con.condition_variable + " ";
            if (hasMinLimit) str += con.condition_min + " 이상 ";
            if (hasMaxLimit) str += con.condition_max + " 이하";
        }

        return str;
    }

    private string GetEffectRange(string minStr, string maxStr)
    {
        string str="";

        float min = StringCalculator.Calculate(minStr);
        float max = StringCalculator.Calculate(maxStr);

        str+= string.Format("{0:+0;-0}",min);
        if(Mathf.Abs(min - max) > 0.0001f) str+= "~" + string.Format("{0:+0;-0}", max);

        return str;
    }
}
