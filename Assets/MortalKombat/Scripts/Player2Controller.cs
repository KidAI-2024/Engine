using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Controls
{
    public string forward;
    public string backward;
    public string jump;
    public string primaryHit;
    public string secondaryHit;
    public string block;
}
public class Player2Controller : MonoBehaviour
{
    public int health;
    public int primaryPower;
    public int secondaryPower;
    public float speed; // Adjust the speed as needed
    public Controls controls;


    Animator animator;
    int isWalkingHash;
    int isWalkingBackHash;
    int secondaryHitHash;
    int primaryHitHash;
    // int jumpHash;
    void OnEnable() // instead of start to make sure the health is set when the game is starts
    {
        health = 150;
        primaryPower = 20;
        secondaryPower = 15;
        speed = 1.5f;
        controls = new Controls();
        controls.forward = "left";
        controls.backward = "right";
        controls.jump = "up";
        controls.primaryHit = "/";
        controls.secondaryHit = ".";
        controls.block = "down";
        
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isWalkingBackHash = Animator.StringToHash("back");
        secondaryHitHash = Animator.StringToHash("secondary");
        primaryHitHash = Animator.StringToHash("primary");
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
        bool legPunsh = animator.GetBool(secondaryHitHash);
        bool boxPunsh = animator.GetBool(primaryHitHash);
        // bool jump = animator.GetBool(jumpHash);

        // check the name of the current animation and its state
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
        if (secondaryHitPressed)
        {
            animator.SetBool(secondaryHitHash, true);
        }
        ResetAnimation(secondaryHitHash,"secondary");

        // Melee Punsh
        if (primaryHitPressed)
        {
            animator.SetBool(primaryHitHash, true);
        }
        ResetAnimation(primaryHitHash,"primary");
        
        // Move the character forward if walking
        if (transform.position.z < 33.8f && transform.position.z >= 28.2f)
        {
            if (isWalking)
            {
                if (forwardPressed)
                {
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                }
                else if (backwardPressed)
                {
                    transform.Translate(Vector3.back * speed * Time.deltaTime);
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
