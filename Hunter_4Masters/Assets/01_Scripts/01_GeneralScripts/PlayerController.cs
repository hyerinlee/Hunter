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
        //공격 애니메이션 설정
        //아직 공격애니메이션 없어서 공격 애니메이션 대신 넣었음
        StartCoroutine(AttackCoroutine());
        // Enemy.OnDamaged(10.0f);
    }

    IEnumerator AttackCoroutine()
    {
        anim.SetBool("isAttack", true);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("처맞는넘" + enemy.name);
            enemy.GetComponent<EnemyMove>().OnDamaged(10);
        }
            

        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isAttack", false);
    }
}
