using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // public void Move()
    // {
    //     Debug.Log("Move");
    // }

    // public void Attack()
    // {
    //     Debug.Log("Attack");
    // }

    // public void Damaged()
    // {
    //     Debug.Log("Damaged");
    // }  

    // public void Die()
    // {
    //     Debug.Log("Die");
    // }
}

public class AttackRange
{
    public float range_x;
    public float range_y;
}