using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public string name;
    public float hp, atk, aps, def, spd;
    public List<AttackRange> attackRange;
    public float moveLimit;

    public Rigidbody2D rigid;
    public SpriteRenderer sr;
    public Animator anim;
    public CapsuleCollider2D collider;

    public int nextMove;

    public Image nowHbBar;
    public float maxHp;

    public GameObject prfHpBar;
    public GameObject canvas;
    //public float height = 1.7f;

    public RectTransform hpBar;
    public GameObject hpPoolObj;

    public void MakeHpBar()
    {
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        maxHp = hp;
        
        // //오브젝트풀링구현
        // hpPoolObj = ObjectPoolManager.Instance.pool.Pop();
        // hpBar = hpPoolObj.GetComponent<RectTransform>();
        // hpBar.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, 0);

        //ObjectPoolManager.Instance.pool.Pop().transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, 0);

        nowHbBar = hpBar.transform.GetChild(0).GetComponent<Image>();

        UpdateHpBarPosition();
    }

    public void UpdateHpBarPosition()
    {
        //Vector3 _hpBarPos = Camera.main.WorldToViewportPoint(new Vector3(transform.position.x * offset, transform.position.y * 10 + height, 0));
        Vector3 _hpBarPos = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, 0);
        hpBar.position = _hpBarPos;
        nowHbBar.fillAmount = (float)hp / (float)maxHp;
    }

    public void MakeNameTag()
    {
        canvas = GameObject.Find("Canvas");
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent<RectTransform>();
        maxHp = hp;
    }

    public virtual void Move()
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
            Invoke("ChangeDirection", 3);
        }

        // player Check
        Vector3 rayDirection;
        if(nextMove != 1) rayDirection = Vector3.left;
        else rayDirection = Vector3.right;
        RaycastHit2D playerRayHit = Physics2D.Raycast(frontVec, rayDirection, 0.5f, LayerMask.GetMask("Player"));
    }

    public virtual void Skill(){}

    public void ChangeDirection()
    {
        //set Next Active
        nextMove  = Random.Range(-1, 2);

        //Recursive
        float nextChangingTime = Random.Range(1f, 3f);
        Invoke("ChangeDirection", nextChangingTime);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("공격!");
        if(!canvas)
            MakeHpBar();
            
        if(col.CompareTag("Player"))
        {
            hp -= 1;
            if(hp < 0)
                Die();
        }
    }

    public void Die()
    {
        //hpPoolObj.GetComponent<HpBar>().PushHpBar();
        Destroy(hpBar.gameObject);
        Destroy(this.gameObject);
    }
}

public class AttackRange
{
    public float range_x;
    public float range_y;
}