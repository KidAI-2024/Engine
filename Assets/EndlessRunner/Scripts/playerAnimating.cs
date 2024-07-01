using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimating : MonoBehaviour
{
    Animator animator;
    private playerMovement pm;
    void Start()
    {
        animator = GetComponent<Animator>();
        pm=GetComponent<playerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        print("in hell");
        Debug.Log(pm.predicted_control);
        
        if (Input.GetKeyDown(KeyCode.Space)||pm.predicted_control=="Jump")
        {
            Debug.Log("predicted control is jump "+pm.predicted_control);
            animator.SetBool("jump", true);
        }
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("jump", false);
        }
    }
}
