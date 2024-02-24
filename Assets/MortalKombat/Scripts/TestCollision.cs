using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound;
    
    Player1Controller Player1;
    Player2Controller Player2;
    Animator PLayer1Animator;
    Animator PLayer2Animator;

    int boxHash;
    int legHash;
    int winHash;
    int dieHash;
    int hitHash;

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
    }
    
    void OnTriggerEnter (Collider col)
    {   
        // player (left player)
        if (PLayer1Animator.GetBool(boxHash) && this.gameObject.tag == "hand" && col.gameObject.tag == "enemy")
        {
            Player2.health -= Player1.boxPower;
            // play hit animation
            StartCoroutine(Player2HitAnimation());

            // reset box animation
            PLayer1Animator.SetBool(boxHash, false);

            // play hit sound
            audioSource.PlayOneShot(hitSound);
        }
        if (PLayer1Animator.GetBool(legHash) && this.gameObject.tag == "leg" && col.gameObject.tag == "enemy")
        {
            Player2.health -= Player1.legPower;
            // play hit animation
            StartCoroutine(Player2HitAnimation());
            
            // reset legPunsh animation
            PLayer1Animator.SetBool(legHash, false);

            // play hit sound
            audioSource.PlayOneShot(hitSound);
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
            PLayer2Animator.SetBool(boxHash, false);
            StartCoroutine(Player1HitAnimation());
            audioSource.PlayOneShot(hitSound);
        }
        if (PLayer2Animator.GetBool(legHash) && this.gameObject.tag == "leg" && col.gameObject.tag == "player")
        {
            Player1.health -= Player2.legPower;
            StartCoroutine(Player1HitAnimation());
            PLayer2Animator.SetBool(legHash, false);
            audioSource.PlayOneShot(hitSound);
        }
        if (Player1.health <= 0)
        {
            PLayer2Animator.SetBool(winHash, true);
            PLayer1Animator.SetBool(dieHash, true);
        }
    }

    IEnumerator Player1HitAnimation()
    {
        PLayer1Animator.SetBool(hitHash, true);
        yield return new WaitForSeconds(0.5f);
        PLayer1Animator.SetBool(hitHash, false);
    }
    IEnumerator Player2HitAnimation()
    {
        PLayer2Animator.SetBool(hitHash, true);
        yield return new WaitForSeconds(0.5f);
        PLayer2Animator.SetBool(hitHash, false);
    }
}
