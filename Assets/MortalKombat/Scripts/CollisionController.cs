using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat
{
    public class CollisionController : MonoBehaviour
    {   
        Player1Controller Player1;
        Player1Controller Player2;
        Animator PLayer1Animator;
        Animator PLayer2Animator;

        int primaryHitHash;
        int secondaryHitHash;
        int winHash;
        int dieHash;
        int hitHash;
        int blockHash;

        void Start()
        {
            GameObject p1 = GameObject.Find("Player1");
            GameObject p2 = GameObject.Find("Player2");
            Player1 = p1.GetComponent<Player1Controller>();
            Player2 = p2.GetComponent<Player1Controller>();
            PLayer1Animator = p1.GetComponent<Animator>();
            PLayer2Animator = p2.GetComponent<Animator>();

            winHash = Animator.StringToHash("win");
            dieHash = Animator.StringToHash("die");
            hitHash = Animator.StringToHash("hit");
            blockHash = Animator.StringToHash("block");
            primaryHitHash = Animator.StringToHash("primary");
            secondaryHitHash = Animator.StringToHash("secondary");
        }
        
        void OnTriggerEnter (Collider col)
        {   
            if(PLayer1Animator == null || PLayer2Animator == null)
            {
                return;
            }
            // player (left player)
            if (PLayer1Animator.GetBool(primaryHitHash) && this.gameObject.tag == "primary1" && col.gameObject.tag == "enemy")
            {
                Player2.health -= Player1.primaryPower;
                PLayer2Animator.SetBool(hitHash, true);
                PLayer1Animator.SetBool(primaryHitHash, false);
            }
            if (PLayer1Animator.GetBool(secondaryHitHash) && this.gameObject.tag == "secondary1" && col.gameObject.tag == "enemy")
            {
                Player2.health -= Player1.secondaryPower;
                PLayer2Animator.SetBool(hitHash, true);
                PLayer1Animator.SetBool(secondaryHitHash, false);
            }
            if (Player2.health <= 0)
            {
                PLayer1Animator.SetBool(winHash, true);
                PLayer2Animator.SetBool(dieHash, true);
            }

            // enemy (right player)
            if (PLayer2Animator.GetBool(primaryHitHash) && this.gameObject.tag == "primary2" && col.gameObject.tag == "player")
            {
                Player1.health -= Player2.primaryPower;
                PLayer1Animator.SetBool(hitHash, true);
                PLayer2Animator.SetBool(primaryHitHash, false);
            }
            if (PLayer2Animator.GetBool(secondaryHitHash) && this.gameObject.tag == "secondary2" && col.gameObject.tag == "player")
            {
                Player1.health -= Player2.secondaryPower;
                PLayer1Animator.SetBool(hitHash, true);
                PLayer2Animator.SetBool(secondaryHitHash, false);
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