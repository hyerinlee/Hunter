using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    private string name;
    private int hp;
    private int ATK;
    private int APS;
    private float[] Attack_Arrange = new float[2];
    private int DEF;
    private int SPD;
    private float Move_Limit;

    void Move(){}
    void Detect(){}
    void Attack(){}
    void Damaged(){}
    void Die(){}
}
