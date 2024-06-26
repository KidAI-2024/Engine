using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat
{
    public struct Controls
    {
        public List<string> forward;
        public List<string> backward;
        public List<string> primaryHit;
        public List<string> secondaryHit;
        public string jump;
        public string block;
        public bool isEnabled;
    }
    public class Player1Controller : MonoBehaviour
    {
        public int maxHealth;
        public int health;
        public int primaryPower;
        public int secondaryPower;
        public float speed; // Adjust the speed as needed
        public string prediction = "";
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

        // for automatic controlling the second player
        public bool forwardAuto = false;
        public bool backwardAuto = false;
        public bool primaryHitAuto = false;
        public bool secondaryHitAuto = false;

        GameManager gameManager;
        void OnEnable() // instead of start to make sure the health is set when the game is starts
        {
            gameManager = GameManager.Instance;
            // Assuming the Animator component is attached to the child GameObject as this script
            animator  = GetComponentInChildren<Animator>();
            isWalkingHash = Animator.StringToHash("isWalking");
            secondaryHitHash = Animator.StringToHash("secondary");
            primaryHitHash = Animator.StringToHash("primary");
            jumpHash = Animator.StringToHash("jump");
            jumpKickHash = Animator.StringToHash("JumpKick");
            blockHash = Animator.StringToHash("block");
        }

        void Update()
        {
            // Disable input if the 3..2..1 countdown is still running
            if(!UI.IsInputEnabled || !controls.isEnabled){
                return;
            }

            bool isWalking = animator.GetBool(isWalkingHash);
            bool legPunsh = animator.GetBool(secondaryHitHash);
            bool boxPunsh = animator.GetBool(primaryHitHash);
            bool jump = animator.GetBool(jumpHash);
            bool block = animator.GetBool(blockHash);

            bool forwardPressed = Input.GetKey(controls.forward[0]) || prediction == controls.forward[1] || forwardAuto;
            bool backwardPressed = Input.GetKey(controls.backward[0]) || prediction == controls.backward[1] || backwardAuto;
            bool primaryHitPressed = Input.GetKeyDown(controls.primaryHit[0]) || prediction == controls.primaryHit[1] || primaryHitAuto;
            bool secondaryHitPressed = Input.GetKeyDown(controls.secondaryHit[0]) || prediction == controls.secondaryHit[1] || secondaryHitAuto;
            bool jumpPressed = Input.GetKeyDown(controls.jump);
            bool blockPressed = Input.GetKeyDown(controls.block);

            // Box Punsh
            if (primaryHitPressed)
            {
                animator.SetBool(primaryHitHash, true);
            }
            ResetAnimation(primaryHitHash,"primary");

            // Leg Punsh
            if (secondaryHitPressed && !jumpPressed && !animator.GetCurrentAnimatorStateInfo(0).IsName("JumpKick"))
            {
                animator.SetBool(secondaryHitHash, true);
            }
            ResetAnimation(secondaryHitHash,"secondary");


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


            // Move the character forward if walking
            if (isWalking)
            {
                bool isPlayer1 = gameObject.name == "Player1";
                bool isForestMap = gameManager.mapName == "Forest";

                // Set limits based on the map
                float forwardLimit = isForestMap ? endLimit : startLimit;
                float backwardLimit = isForestMap ? startLimit : endLimit;

                bool isInForwardLimit;
                bool isInBackwardLimit; 
                if (isForestMap){
                    isInForwardLimit = isPlayer1 ? transform.position.z > endLimit : transform.position.z < endLimit;
                    isInBackwardLimit = isPlayer1 ? transform.position.z < startLimit : transform.position.z > startLimit;
                }
                else{
                    isInForwardLimit = isPlayer1 ? transform.position.z < endLimit : transform.position.z > endLimit;
                    isInBackwardLimit = isPlayer1 ? transform.position.z > startLimit : transform.position.z < startLimit;
                }
                Quaternion forwardRotation = isForestMap 
                    ? (isPlayer1 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0))
                    :  Quaternion.Euler(0, 0, 0);

                Quaternion backwardRotation = isForestMap 
                    ? (isPlayer1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0))
                    : Quaternion.Euler(0, 180, 0);

                if (forwardPressed && isInForwardLimit && !backwardPressed)
                {
                    // rotate the character to the direction of movement
                    transform.rotation = forwardRotation;
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
                else if (backwardPressed && isInBackwardLimit && !forwardPressed)
                {
                    // rotate the character to the direction of movement
                    transform.rotation = backwardRotation;
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
}