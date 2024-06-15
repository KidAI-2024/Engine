using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Survival
{
    public class Sway : MonoBehaviour
    {
        // Determines how much the object (e.g., a pistol) will sway in response to mouse movement.
        public float swayAmount = 0.5f;

        // Controls how smoothly the sway motion transitions.
        public float smoothFactor = 2f;      

        // Stores the initial rotation of the object to which the script is attached.
        private Quaternion initialRotation;
        // A reference to the player's camera transform.
        private Transform playerCamera;

        void Start()
        {
            playerCamera = Camera.main.transform; //MAIN CAMERA
            
            initialRotation = transform.localRotation; //initial rotation is the current rotation of the object
        }

        void Update()
        {
            float inputX = -Input.GetAxis("Mouse X") * swayAmount;
            float inputY = -Input.GetAxis("Mouse Y") * swayAmount;

            Quaternion targetRotation = Quaternion.Euler(inputY, inputX, 0f) * initialRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothFactor);
        }
    }

}