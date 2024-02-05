using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    Player1Controller Player1;
    Player2Controller Player2;
    Animator PLayer1Animator;
    Animator PLayer2Animator;

    void Start()
    {
        GameObject p1 = GameObject.Find("Player1");
        GameObject p2 = GameObject.Find("Player2");
        Player1 = p1.GetComponent<Player1Controller>();
        Player2 = p2.GetComponent<Player2Controller>();
        PLayer1Animator = p1.GetComponent<Animator>();
        PLayer2Animator = p2.GetComponent<Animator>();
    }
    void OnTriggerEnter (Collider col)
    {   
        // player
        if (PLayer1Animator.GetBool(Animator.StringToHash("box")) && this.gameObject.tag == "hand" && col.gameObject.tag == "enemy")
        {
            Player2.health -= 9;
        }
        if (PLayer1Animator.GetBool(Animator.StringToHash("legPunsh")) && this.gameObject.tag == "leg" && col.gameObject.tag == "enemy")
        {
            Player2.health -= 7;
        }
        if (Player2.health <= 0)
        {
            PLayer2Animator.SetBool(Animator.StringToHash("die"), true);
        }

        // enemy
        if (PLayer2Animator.GetBool(Animator.StringToHash("box")) && this.gameObject.tag == "weapon" && col.gameObject.tag == "player")
        {
            Player1.health -= 15;
        }
        if (PLayer2Animator.GetBool(Animator.StringToHash("legPunsh")) && this.gameObject.tag == "leg" && col.gameObject.tag == "player")
        {
            
            Player1.health -= 10;
        }
        if (Player1.health <= 0)
        {
            PLayer1Animator.SetBool(Animator.StringToHash("die"), true);
        }
        
    }
}
