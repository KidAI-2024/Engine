using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat
{
    public struct Controls
    {
        public string forward;
        public string backward;
        public string jump;
        public string primaryHit;
        public string secondaryHit;
        public string block;
    }
    public class Player1Controller : MonoBehaviour
    {
        public int health;
        public int primaryPower;
        public int secondaryPower;
        public float speed; // Adjust the speed as needed
        public Controls controls;   

        Animator animator;
        int isWalkingHash;
        int secondaryHitHash;
        int primaryHitHash;
        int jumpHash;
        int jumpKickHash;
        int blockHash;
        public float startLimit;
        public float endLimit;

        

        void OnEnable() // instead of start to make sure the health is set when the game is starts
        {
            // Assuming the Animator component is attached to the child GameObject as this script
            animator  = GetComponentInChildren<Animator>();
            isWalkingHash = Animator.StringToHash("isWalking");
            secondaryHitHash = Animator.StringToHash("secondary");
            primaryHitHash = Animator.StringToHash("primary");
            jumpHash = Animator.StringToHash("jump");
            jumpKickHash = Animator.StringToHash("JumpKick");
            blockHash = Animator.StringToHash("block");

            Debug.Log(gameObject.name + " " + startLimit + " " + endLimit);
        }

        void Update()
        {
            // Disable input if the 3..2..1 countdown is still running
            if(!UI.IsInputEnabled){
                return;
            }

            bool isWalking = animator.GetBool(isWalkingHash);
            bool legPunsh = animator.GetBool(secondaryHitHash);
            bool boxPunsh = animator.GetBool(primaryHitHash);
            bool jump = animator.GetBool(jumpHash);
            bool block = animator.GetBool(blockHash);

            bool forwardPressed = Input.GetKey(controls.forward);
            bool backwardPressed = Input.GetKey(controls.backward);
            bool jumpPressed = Input.GetKeyDown(controls.jump);
            bool primaryHitPressed = Input.GetKeyDown(controls.primaryHit);
            bool secondaryHitPressed = Input.GetKeyDown(controls.secondaryHit);
            bool blockPressed = Input.GetKeyDown(controls.block);

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
            if (blockPressed)
            {
                animator.SetBool(blockHash, true);
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                animator.SetBool(blockHash, false);
            }

            // Jump & Kick
            if (jumpPressed && secondaryHitPressed)
            {
                animator.SetBool(jumpKickHash, true);
            }
            ResetAnimation(jumpKickHash,"JumpKick");

            // Jump
            if (jumpPressed && !secondaryHitPressed && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpKick"))
            {
                animator.SetBool(jumpHash, true);
            }
            ResetAnimation(jumpHash,"Jumping");

            // Leg Punsh
            if (secondaryHitPressed && !jumpPressed && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpKick"))
            {
                animator.SetBool(secondaryHitHash, true);
            }
            ResetAnimation(secondaryHitHash,"secondary");


            // Box Punsh
            if (primaryHitPressed)
            {
                animator.SetBool(primaryHitHash, true);
            }
            ResetAnimation(primaryHitHash,"primary");

            // Move the character forward if walking
            if (isWalking)
            {
                bool isInForwardLimit = gameObject.name == "Player1" ? transform.position.z > endLimit : transform.position.z < endLimit;
                bool isInBackwardLimit = gameObject.name == "Player1" ? transform.position.z < startLimit : transform.position.z > startLimit;
                if (forwardPressed && isInForwardLimit && !backwardPressed)
                {
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
                else if (backwardPressed && isInBackwardLimit && !forwardPressed)
                {
                    transform.Translate(Vector3.back * speed * Time.deltaTime);
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
}