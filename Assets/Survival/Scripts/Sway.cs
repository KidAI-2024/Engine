/*
Purpose of the Script
The purpose of this script is to create a swaying effect on a game object (e.g., a weapon like a pistol) in response to the player's mouse movement. 
This is often used in first-person shooter games to enhance realism by making the weapon follow the player's camera movement smoothly.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survival
{
    public class Sway : MonoBehaviour
    {
        // Public Variables
        // Determines how much the object (e.g., a pistol) will sway in response to mouse movement.
        public float swayAmount = 0.5f;

        // Controls how smoothly the sway motion transitions.
        public float smoothFactor = 2f;

        // Private Variables
        // Stores the initial rotation of the object to which the script is attached.
        private Quaternion initialRotation;

        // A reference to the player's camera transform.
        private Transform playerCamera;

        void Start()
        {
            // Get the transform of the main camera.
            playerCamera = Camera.main.transform;

            // Store the initial local rotation of the object.
            initialRotation = transform.localRotation;
        }

        void Update()
        {
            // Get the mouse input on the X and Y axes, inverted for swaying effect.
            /*
            Input.GetAxis("Mouse X") and Input.GetAxis("Mouse Y") retrieve the mouse movement along the X and Y axes respectively. 
            The negative sign (-) is applied to invert these values, likely for a swaying effect.
            */
            float inputX = -Input.GetAxis("Mouse X") * swayAmount;
            float inputY = -Input.GetAxis("Mouse Y") * swayAmount;

            /*
            Quaternion.Euler(inputY, inputX, 0f) creates a rotation Quaternion based on the mouse input (inputX for X-axis, inputY for Y-axis, and 0f for Z-axis). 
            This represents the desired rotation change.
            */
            // Calculate the target rotation based on the mouse input and initial rotation.
            Quaternion targetRotation = Quaternion.Euler(inputY, inputX, 0f) * initialRotation;

            // Smoothly interpolate between the current rotation and the target rotation.
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothFactor);
            /*
            Time.deltaTime * smoothFactor controls the speed of interpolation, 
            where Time.deltaTime is the time in seconds it took to complete the last frame and smoothFactor is a float variable to adjust the smoothness of the rotation.
            */
        }
    }
}
