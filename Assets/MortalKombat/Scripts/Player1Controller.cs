using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{
    public int health;
    public int legPower = 10;
    public int boxPower = 15;

    Animator animator;
    int isWalkingHash;
    int legPunshHash;
    int boxPunshHash;
    int jumpHash;
    int jumpKickHash;
    int blockHash;
    public float moveSpeed = 0.5f; // Adjust the speed as needed

    void Start()
    {
        health = 100;
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        legPunshHash = Animator.StringToHash("legPunsh");
        boxPunshHash = Animator.StringToHash("box");
        jumpHash = Animator.StringToHash("jump");
        jumpKickHash = Animator.StringToHash("JumpKick");
        blockHash = Animator.StringToHash("block");
    }

    void Update()
    {
        // Disable input if the 3..2..1 countdown is still running
        if(!UI.IsInputEnabled){
            return;
        }

        bool isWalking = animator.GetBool(isWalkingHash);
        bool legPunsh = animator.GetBool(legPunshHash);
        bool boxPunsh = animator.GetBool(boxPunshHash);
        bool jump = animator.GetBool(jumpHash);
        bool block = animator.GetBool(blockHash);

        bool forwardPressed = Input.GetKey("d");
        bool backwardPressed = Input.GetKey("a");
        bool jumpPressed = Input.GetKeyDown(KeyCode.W);
        bool legKickPressed = Input.GetKeyDown(KeyCode.Space);

        // Walking
        if (!isWalking && (forwardPressed || backwardPressed))
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !(forwardPressed || backwardPressed))
        {
            animator.SetBool(isWalkingHash, false);
        }
        
        // Block
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetBool(blockHash, true);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            animator.SetBool(blockHash, false);
        }

        // Jump & Kick
        if (jumpPressed && legKickPressed)
        {
            animator.SetBool(jumpKickHash, true);
        }
        ResetAnimation(jumpKickHash,"JumpKick");

        // Jump
        if (jumpPressed && !legKickPressed && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpKick"))
        {
            animator.SetBool(jumpHash, true);
        }
        ResetAnimation(jumpHash,"Jumping");

        // Leg Punsh
        if (legKickPressed && !jumpPressed && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpKick"))
        {
            animator.SetBool(legPunshHash, true);
        }
        ResetAnimation(legPunshHash,"legPunsh");


        // Box Punsh
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool(boxPunshHash, true);
        }
        ResetAnimation(boxPunshHash,"Boxing");

        // check if the player is not at the end of the screen
        if (transform.position.z < 33.8f && transform.position.z >= 28.9f)
        {
            // Move the character forward if walking
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
        if(animator.GetBool(hash) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            animator.SetBool(hash, false);
        }
    }
}
