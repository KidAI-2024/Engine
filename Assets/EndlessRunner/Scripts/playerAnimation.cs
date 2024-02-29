using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            animator.SetBool("jump", true);

        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            animator.SetBool("jump", false);
        }
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    animator.SetBool("right", true);
        //}
        //if (!Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    animator.SetBool("right", false);
        //}
    }
}
