using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    private playerMovement pm;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        pm = GetComponent<playerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("run", true);
        }
        //if (Input.GetKeyDown(KeyCode.UpArrow) || pm.predicted_control == "Jump")
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("in jump");
            animator.SetBool("jump", true);

        }
        //if (Input.GetKeyUp(KeyCode.UpArrow)||pm.predicted_control!="Jump")
        if (Input.GetKeyUp(KeyCode.UpArrow)&& pm.predicted_control != "Jump")
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
