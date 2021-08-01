using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;
    public int nextMove;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Think()
    {
        //set Next Active
        nextMove  = Random.Range(-1, 2);

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Move()
    {
        // move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.4f, 0.05f), 0f, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        if(rayHit.collider == null){
            Turn();
        }
        else
            anim.SetBool("isAttack", false);

        // player Check
        Vector3 rayDirection;
        if(nextMove != 1) rayDirection = Vector3.left;
        else rayDirection = Vector3.right;
        RaycastHit2D playerRayHit = Physics2D.Raycast(frontVec, rayDirection, 0.5f, LayerMask.GetMask("Player"));

        if(playerRayHit.collider != null){
            Attack(rayHit);
        }
    }

    void Attack(RaycastHit2D rayHit)
    {
        if(rayHit.distance < 0.3f){
            rigid.AddForce(Vector2.up * 0.5f, ForceMode2D.Impulse);
            anim.SetBool("isAttack", true);
        }
    }

    void Turn()
    {
        nextMove *= -1;
        sr.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 3);
    }
}
