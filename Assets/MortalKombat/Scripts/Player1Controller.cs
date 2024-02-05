using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Controller : MonoBehaviour
{
    Animator animator;
    public int health;
    int isWalkingHash;
    int legPunshHash;
    int boxPunshHash;
    int jumpHash;
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
        
        // Block
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Set the "jump" parameter to true
            animator.SetBool(blockHash, true);
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            // Set the "jump" parameter to false
            animator.SetBool(blockHash, false);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Set the "jump" parameter to true
            animator.SetBool(jumpHash, true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            // Set the "jump" parameter to false
            animator.SetBool(jumpHash, false);
        }


        // Leg Punsh
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set the "legPunsh" parameter to true
            animator.SetBool(legPunshHash, true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Set the "legPunsh" parameter to false
            // wait for the animation to finish then make it false
            StartCoroutine(Reset(legPunshHash));
        }


        // Box Punsh
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Set the "box" parameter to true
            animator.SetBool(boxPunshHash, true);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            // Set the "box" parameter to false
            // wait for the animation to finish then make it false
            StartCoroutine(Reset(boxPunshHash));
        }

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
    IEnumerator Reset(int hash)
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(hash, false);
    }
}
