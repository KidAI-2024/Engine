using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    public float moveSpeed = 0.5f; // Adjust the speed as needed

    void Start()
    {
        // Assuming the Animator component is attached to the child GameObject as this script
        animator  = GetComponentInChildren<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("left");
        bool backwardPressed = Input.GetKey("right");

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
