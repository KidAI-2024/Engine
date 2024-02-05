using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaAnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int legPunshHash;
    int boxPunshHash;
    public float moveSpeed = 0.5f; // Adjust the speed as needed

    void Start()
    {
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        legPunshHash = Animator.StringToHash("legPunsh");
        boxPunshHash = Animator.StringToHash("box");
    }

    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool legPunsh = animator.GetBool(legPunshHash);
        bool boxPunsh = animator.GetBool(boxPunshHash);
        bool forwardPressed = Input.GetKey("w");
        bool backwardPressed = Input.GetKey("s");

        // Check if the "W" key is pressed
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Set the "legPunsh" parameter to true
            animator.SetBool(legPunshHash, true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Set the "legPunsh" parameter to false
            animator.SetBool(legPunshHash, false);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Set the "box" parameter to true
            animator.SetBool(boxPunshHash, true);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            // Set the "box" parameter to false
            animator.SetBool(boxPunshHash, false);
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
}
