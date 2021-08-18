using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public string name;
    public int hp;
    public int atk;
    public int aps;
    public int def;
    public int spd;
    public List<AttackRange> attackRange;
    public float moveLimit;

    public Rigidbody2D rigid;
    public SpriteRenderer sr;
    public Animator anim;

    public int nextMove;

    public Image nowHbBar;
    public int maxHp;

    public GameObject prfHpBar;
    public GameObject canvas;
    public float height = 1.7f;

    public RectTransform hpBar;
    public int offset = 10; // 이걸 무슨 값으로 해야하는건지 모르겠음. 카메라 크기와 관련이 있을 것 같은데

    public void MakeHpBar()
    {
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        maxHp = hp;

        nowHbBar = hpBar.transform.GetChild(0).GetComponent<Image>();
    }

    public void UpdateHpBarPosition()
    {
        Vector3 _hpBarPos = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x * offset, transform.position.y * 10 + height, 0));
        hpBar.position = _hpBarPos;
        nowHbBar.fillAmount = (float)hp / (float)maxHp;

    }

    public void Move()
    {
        // move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.4f, 0.05f), 0f, Vector2.down, 0.7f, LayerMask.GetMask("Ground"));
        if(rayHit.collider == null){
            nextMove *= -1;
            sr.flipX = nextMove == 1;

            CancelInvoke();
            Invoke("Think", 3);
        }

        // player Check
        Vector3 rayDirection;
        if(nextMove != 1) rayDirection = Vector3.left;
        else rayDirection = Vector3.right;
        RaycastHit2D playerRayHit = Physics2D.Raycast(frontVec, rayDirection, 0.5f, LayerMask.GetMask("Player"));
    }

    void Think()
    {
        //set Next Active
        nextMove  = Random.Range(-1, 2);

        //Recursive
        float nextThinkTime = Random.Range(1f, 3f);
        Invoke("Think", nextThinkTime);
    }

    // public void Attack()
    // {
    //     Debug.Log("Attack");
    // }

    // private void OnTriggerEneter2D(Collider2D col)
    // {
    //     if(col.CompareTag("Player"))
    //     {
    //         Debug.Log("슬라임 공격!");
    //     }
    // }

    // public void Damaged()
    // {
    //     Debug.Log("Damaged");
    // }  

    public void Die()
    {
        Destroy(hpBar.gameObject);
        Destroy(this.gameObject);
    }
}

public class AttackRange
{
    public float range_x;
    public float range_y;
}