using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Monster
{
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        MakeHpBar();

        Invoke("Think", 1);
    }

    void FixedUpdate()
    {
        Move();
        UpdateHpBarPosition();
    }

}