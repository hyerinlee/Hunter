using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    //private float maxHp;
    float jumpTime = 0;
    int isJumping = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        ChangeDirection();
    }

    void FixedUpdate()
    {
        Move();

        // hpBar가 활성화 되어 있는 경우 hpBar도 업데이트
        if(canvas) UpdateHpBarPosition();
    }

    public override void Move()
    {
        // move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.4f, 0.05f), 0f, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        jumpTime += Time.deltaTime;
        if(nextMove!=0 && rayHit.collider != null && jumpTime >= 3){
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * 200);
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
            jumpTime = 0; isJumping = 1;
            Debug.Log("점프중");
        }
        if(rayHit.collider != null){
            anim.SetBool("isJumping", false);
            isJumping = 0;
            Debug.Log("점프 애니메이션 해제");
        }
        if(rayHit.collider == null && isJumping == 0){
            nextMove *= -1;
            sr.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("ChangeDirection", 3);
        }

        // player Check
        Vector3 rayDirection;
        if(nextMove != 1) rayDirection = Vector3.left;
        else rayDirection = Vector3.right;
        RaycastHit2D playerRayHit = Physics2D.Raycast(frontVec, rayDirection, 0.5f, LayerMask.GetMask("Player"));
    }

    public override void Skill()
    {
        Debug.Log("점프");
    }
}

//GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);