﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    private int maxHp;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        maxHp = hp;

        MakeHpBar();

        Invoke("Think", 1);
    }

    void FixedUpdate()
    {
        Move();
        UpdateHpBarPosition();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            Debug.Log("슬라임 공격당함!! 222");
            hp -= 1;
            if(hp < 0)
                Die();
        }
    }
}