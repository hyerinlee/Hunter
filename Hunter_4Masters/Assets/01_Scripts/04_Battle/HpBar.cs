using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : Poolable
{
    // public Image nowHbBar;
    // public int maxHp;

    // public GameObject prfHpBar;
    // public GameObject canvas;

    // public RectTransform hpBar;

    // public void MakeHpBar()
    // {
    //     canvas = GameObject.Find("Canvas");
    //     hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
    //     maxHp = hp;

    //     nowHbBar = hpBar.transform.GetChild(0).GetComponent<Image>();

    //     //UpdateHpBarPosition();
    // }
    
    public void PushHpBar()
    {
        GetComponent<ObjectPool>().Push(this);
    }
}