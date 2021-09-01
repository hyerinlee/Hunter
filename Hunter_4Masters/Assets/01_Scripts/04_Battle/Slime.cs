using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    //private float maxHp;
    float jumpDelay = 3; //점프 쿨타임
    float untilJump; //점프까지 남은 시간
    bool isJumping = false;
    float maxHeight = -100;

    void Awake()
    {
        //  컴포넌트 초기화
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();

        //점프 쿨타임 설정
        untilJump = jumpDelay;

        //방향전환 메소드 활성화
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


        //ground check
        Debug.DrawRay(transform.position, new Vector3(0, -1, 0) * 0.5f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, new Vector3(0, -1, 0)*0.5f); 

        // Jump 쿨타임
        untilJump -= Time.deltaTime;

        //Jump
        if(nextMove!=0 && rayHit.collider!=null && (isJumping==false) && (untilJump <= 0)){
            CancelInvoke();
            isJumping = true;
            sr.flipX = nextMove != 1;
            anim.SetTrigger("isUp");
            rigid.AddForce(new Vector3(nextMove, 1, 0) * 200);//Vector2.up
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

            untilJump = jumpDelay;

            Debug.Log("점프중");
        }

        if(transform.position.y > maxHeight)
            maxHeight = transform.position.y;
        if(transform.position.y < maxHeight)
        {
            anim.SetBool("isDown", true);
            maxHeight = transform.position.y;
        }
            


        //지형 끝에 다다랐을 때
        if(rayHit.collider == null){
            nextMove *= -1;
            sr.flipX = nextMove == 1;
            CancelInvoke();
            Invoke("ChangeDirection", 3);
        }

        // player Check
        Vector3 rayDirection;
        if(nextMove != 1) rayDirection = Vector3.left;
        else rayDirection = Vector3.right;
        //RaycastHit2D playerRayHit = Physics2D.Raycast(frontVec, rayDirection, 0.5f, LayerMask.GetMask("Player"));
    }

    public override void Skill()
    {
        Debug.Log("점프");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("착지");
            isJumping = false;
            nextMove = 0;
            anim.SetBool("isDown", false);
            anim.SetTrigger("isLanding");
            sr.flipX = nextMove == 1;
            Invoke("ChangeDirection", 1);
        }
    }
}