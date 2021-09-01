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
        collider = GetComponent<CapsuleCollider2D>();

        ChangeDirection();
    }

    void FixedUpdate()
    {
        Move();

        // hpBar가 활성화 되어 있는 경우 hpBar도 업데이트
        if(canvas) UpdateHpBarPosition();
    }

    public override void Skill()
    {
        Debug.Log("칼 휘두르기");
    }

}