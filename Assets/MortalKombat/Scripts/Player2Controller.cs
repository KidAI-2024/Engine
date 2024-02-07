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

    void Start()
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
        if(!UI.IsInputEnabled){
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
            // Set the "isWalking" parameter to true
            animator.SetBool(isWalkingHash, true);
        }
        if (isWalking && !(forwardPressed || backwardPressed))
        {
            // Set the "isWalking" parameter to false
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
            // Set the "isWalkingBack" parameter to false
            animator.SetBool(isWalkingBackHash, false);
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
        */
        // Leg Punsh with / key
        if (Input.GetKeyDown("/"))
        {
            // Set the "legPunsh" parameter to true
            animator.SetBool(legPunshHash, true);
        }
        if (Input.GetKeyUp("/"))
        {
            // wait for the animation to finish then make it false
            StartCoroutine(Reset(legPunshHash));
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
            // wait for the animation to finish then make it false
            StartCoroutine(Reset(boxPunshHash));
        }
        
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

    IEnumerator Reset(int hash)
    {
        yield return new WaitForSeconds(1.2f);
        animator.SetBool(hash, false);
    }
}
