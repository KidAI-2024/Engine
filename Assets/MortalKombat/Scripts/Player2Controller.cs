using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    // int legPunshHash;
    // int boxPunshHash;
    // int jumpHash;
    public float moveSpeed = 0.5f; // Adjust the speed as needed

    void Start()
    {
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        // legPunshHash = Animator.StringToHash("legPunsh");
        // boxPunshHash = Animator.StringToHash("box");
        // jumpHash = Animator.StringToHash("jump");
    }

    void Update()
    {
        // Disable input if the 3..2..1 countdown is still running
        if(!Timer.IsInputEnabled){
            return;
        }

        bool isWalking = animator.GetBool(isWalkingHash);
        // bool legPunsh = animator.GetBool(legPunshHash);
        // bool boxPunsh = animator.GetBool(boxPunshHash);
        // bool jump = animator.GetBool(jumpHash);

        bool forwardPressed = Input.GetKey("left");
        bool backwardPressed = Input.GetKey("right");

        // Walking
        if (!isWalking && (forwardPressed || backwardPressed))
        {
            // Set the "isWalking" parameter to true
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !(forwardPressed || backwardPressed))
        {
            // Set the "isWalking" parameter to false
            animator.SetBool(isWalkingHash, false);
        }
        /*
        // Jump with arrow up
        if (Input.GetKeyDown("up"))
        {
            // Set the "jump" parameter to true
            animator.SetBool(jumpHash, true);
        }
        if (Input.GetKeyUp("up"))
        {
            // Set the "jump" parameter to false
            animator.SetBool(jumpHash, false);
        }


        // Leg Punsh with / key
        if (Input.GetKeyDown("/"))
        {
            // Set the "legPunsh" parameter to true
            animator.SetBool(legPunshHash, true);
        }
        if (Input.GetKeyUp("/"))
        {
            // Set the "legPunsh" parameter to false
            animator.SetBool(legPunshHash, false);
        }


        // Box Punsh with . key
        if (Input.GetKeyDown("."))
        {
            // Set the "box" parameter to true
            animator.SetBool(boxPunshHash, true);
        }
        if (Input.GetKeyUp("."))
        {
            // Set the "box" parameter to false
            animator.SetBool(boxPunshHash, false);
        }
        */
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
