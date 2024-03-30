using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
{
    public int health;
    public int legPower = 15;
    public int weaponPower = 20;


    Animator animator;
    int isWalkingHash;
    int isWalkingBackHash;
    int legPunshHash;
    int boxPunshHash;
    // int jumpHash;
    public float moveSpeed = 0.5f; // Adjust the speed as needed

    void OnEnable() // instead of start to make sure the health is set when the game is starts
    {
        health = 150;
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isWalkingBackHash = Animator.StringToHash("back");
        legPunshHash = Animator.StringToHash("legPunsh");
        boxPunshHash = Animator.StringToHash("box");
        // jumpHash = Animator.StringToHash("jump");
    }

    void Update()
    {
        // Disable input if the 3..2..1 countdown is still running
        if(!MortalKombat.UI.IsInputEnabled){
            return;
        }

        bool isWalking = animator.GetBool(isWalkingHash);
        bool isWalkingBack = animator.GetBool(isWalkingBackHash);
        bool legPunsh = animator.GetBool(legPunshHash);
        bool boxPunsh = animator.GetBool(boxPunshHash);
        // bool jump = animator.GetBool(jumpHash);

        // check the name of the current animation and its state

        bool forwardPressed = Input.GetKey("left");
        bool backwardPressed = Input.GetKey("right");

        // Walking
        if (!isWalking && (forwardPressed || backwardPressed))
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !(forwardPressed || backwardPressed))
        {
            animator.SetBool(isWalkingHash, false);
        }

        // Walking Back
        if (!isWalkingBack && (backwardPressed))
        {
            // Set the "isWalkingBack" parameter to true
            animator.SetBool(isWalkingBackHash, true);
        }
        if (isWalkingBack && !(backwardPressed))
        {
            animator.SetBool(isWalkingBackHash, false);
        }   

        // Leg Punsh 
        if (Input.GetKeyDown("/"))
        {
            animator.SetBool(legPunshHash, true);
        }
        ResetAnimation(legPunshHash,"legPunsh");

        // Melee Punsh
        if (Input.GetKeyDown("."))
        {
            animator.SetBool(boxPunshHash, true);
        }
        ResetAnimation(boxPunshHash,"melee");
        
        // Move the character forward if walking
        if (transform.position.z < 33.8f && transform.position.z >= 28.9f)
        {
            if (isWalking)
            {
                if (forwardPressed)
                {
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                }
                else if (backwardPressed)
                {
                    transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
                }
            }
        }
    }


    void ResetAnimation(int hash, string animationName = "")
    {
        if(animator.GetBool(hash) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            animator.SetBool(hash, false);
        }
    }
}
