using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimating : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("jump", true);
        }
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("jump", false);
        }
    }
}
