using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private HunterAssociation building;

    private void Awake()
    {
        building = transform.parent.GetComponent<HunterAssociation>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            building.UseStairs();
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.name == "Player")
    //    {
    //        building.UseStairs();
    //    }
    //}
    
}
