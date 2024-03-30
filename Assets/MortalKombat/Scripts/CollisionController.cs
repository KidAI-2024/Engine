using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat
{
    public class CollisionController : MonoBehaviour
    {   
        Player1Controller Player1;
        Player2Controller Player2;
        Animator PLayer1Animator;
        Animator PLayer2Animator;

        int boxHash;
        int legHash;
        int winHash;
        int dieHash;
        int hitHash;
        int blockHash;

        void Start()
        {
            GameObject p1 = GameObject.Find("Player1");
            GameObject p2 = GameObject.Find("Player2");
            Player1 = p1.GetComponent<Player1Controller>();
            Player2 = p2.GetComponent<Player2Controller>();
            PLayer1Animator = p1.GetComponent<Animator>();
            PLayer2Animator = p2.GetComponent<Animator>();

            boxHash = Animator.StringToHash("box");
            legHash = Animator.StringToHash("legPunsh");
            winHash = Animator.StringToHash("win");
            dieHash = Animator.StringToHash("die");
            hitHash = Animator.StringToHash("hit");
            blockHash = Animator.StringToHash("block");
        }
        
        void OnTriggerEnter (Collider col)
        {   
            // player (left player)
            if (PLayer1Animator.GetBool(boxHash) && this.gameObject.tag == "hand" && col.gameObject.tag == "enemy")
            {
                Player2.health -= Player1.boxPower;
                PLayer2Animator.SetBool(hitHash, true);
                PLayer1Animator.SetBool(boxHash, false);
            }
            if (PLayer1Animator.GetBool(legHash) && this.gameObject.tag == "leg" && col.gameObject.tag == "enemy")
            {
                Player2.health -= Player1.legPower;
                PLayer2Animator.SetBool(hitHash, true);
                PLayer1Animator.SetBool(legHash, false);
            }
            if (Player2.health <= 0)
            {
                PLayer1Animator.SetBool(winHash, true);
                PLayer2Animator.SetBool(dieHash, true);
            }

            // enemy (right player)
            if (PLayer2Animator.GetBool(boxHash) && this.gameObject.tag == "weapon" && col.gameObject.tag == "player")
            {
                Player1.health -= Player2.weaponPower;
                PLayer1Animator.SetBool(hitHash, true);
                PLayer2Animator.SetBool(boxHash, false);
            }
            if (PLayer2Animator.GetBool(legHash) && this.gameObject.tag == "leg" && col.gameObject.tag == "player")
            {
                Player1.health -= Player2.legPower;
                PLayer1Animator.SetBool(hitHash, true);
                PLayer2Animator.SetBool(legHash, false);
            }
            if (Player1.health <= 0)
            {
                PLayer2Animator.SetBool(winHash, true);
                PLayer1Animator.SetBool(dieHash, true);
            }
        }
        void Update()
        {
            if(PLayer2Animator.GetBool(hitHash) && PLayer2Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && PLayer2Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                PLayer2Animator.SetBool(hitHash, false);
            }
            if(PLayer1Animator.GetBool(hitHash) && PLayer1Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.1f && PLayer1Animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                PLayer1Animator.SetBool(hitHash, false);
            }
        }
    }
}