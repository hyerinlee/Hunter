using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Joystick joystick;
    public float moveSpeed;
    public float jumpForce;
    public float jumpSpeed;
    private bool isJumping = false;
    private SpriteRenderer sr;

    public GameObject attackMsg;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        attackMsg.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;
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
        if (!isJumping)
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce);
            isJumping = true;
        }
    }

    public void Attack()
    {
        //공격 애니메이션 설정
        //아직 공격애니메이션 없어서 공격 애니메이션 대신 넣었음
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        attackMsg.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackMsg.SetActive(false);
    }
}
