using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Joystick joystick;
    public float moveSpeed;
    public float jumpForce;
    private SpriteRenderer sr;
    private Vector2 boxCastSize = new Vector2(0.4f, 0.05f);
    private float boxCastMaxDistance = 0.7f;
    Animator anim;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public GameObject attackMsg;

    public bool isAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        attackMsg.SetActive(false);
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void OnDrawGizmos()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));

        Gizmos.color = Color.red;
        if (raycastHit.collider != null)
        {
            Gizmos.DrawRay(transform.position, Vector2.down * raycastHit.distance);
            Gizmos.DrawWireCube(transform.position + Vector3.down * raycastHit.distance, boxCastSize);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.down * boxCastMaxDistance);
        }
    }

    private bool IsOnGround()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, boxCastMaxDistance, LayerMask.GetMask("Ground"));
        return (raycastHit.collider != null);
    }

    public void Move()
    {
        Vector3 offset = new Vector3(joystick.GetHorizontalValue() * moveSpeed * Time.deltaTime, 0, 0);
        // 이동방향에 맞춰 플레이어 스프라이트 뒤집기(정지상태 제외)
        if (offset != Vector3.zero)
        {
            // skeleton - stomach.y 180도 반전(rotation) 필요
            sr.flipX = !(offset.x < 0);
        }

        // 화면 밖 넘어가지 않도록 이동제한
        Vector3 newViewPos = Camera.main.WorldToViewportPoint(this.transform.position + offset);
        newViewPos.x = Mathf.Clamp01(newViewPos.x);
        this.transform.position = Camera.main.ViewportToWorldPoint(newViewPos);
    }

    public void Jump()
    {
        if (IsOnGround())
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
        }
    }

    public void Attack()
    {
        anim.SetTrigger("isAttack");
        //공격트리거: false 일 때에는 collider 충돌해도 enemy에게 데미지 없음
        isAttack = true;

        //무기 트리거인데 혹시 몰라서 남겨둠
        //Collider2D weaponCollider = attackPoint.GetComponent<CapsuleCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        //충돌한 collider가 Enemy일 때
        if(col.CompareTag("Enemy") && isAttack == true)
        {
            //충돌한 Enemy 스크립트 불러오기
            string scriptName = col.gameObject.name.Substring(3);
            Monster script = col.gameObject.GetComponent(scriptName) as Monster;
            script.Damaged(1);
        }
        isAttack = false;
    }

    
}
