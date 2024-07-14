/*
    Script Name: UseChest
    Purpose: This script allows the player to interact with a chest GameObject when they are in proximity and press the designated interact button.

    Components:
    - private GameObject OB: Reference to the chest GameObject.
    - public GameObject handUI: UI element indicating interaction availability with the chest.
    - public GameObject objToActivate: Object to activate when the chest is interacted with.
    - private bool inReach: Flag to track if the player is within reach of the chest.

    Methods:
    - Start(): Initializes references and sets initial states of UI and activation objects.
    - OnTriggerEnter(Collider other): Called when another collider enters the trigger zone of the chest GameObject. Activates hand UI if the collider has a "Reach" tag.
    - OnTriggerExit(Collider other): Called when another collider exits the trigger zone of the chest GameObject. Deactivates hand UI if the collider has a "Reach" tag.
    - Update(): Called every frame. Activates objToActivate and opens the chest animation if the player is in reach and presses the "Interact" button.

    Usage:
    Attach this script to the chest GameObject. Assign handUI and objToActivate in the Inspector. Ensure colliders with "Reach" tag are properly set up for triggering interaction.

    Dependencies:
    - Requires Unity's Collider and Animator components attached to the chest GameObject.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survival
{
    public class UseChest : MonoBehaviour
    {
        private GameObject chestObj; // Reference to the chest GameObject
        public GameObject handImg; // UI element for indicating interaction with the chest
        public GameObject objInChest; // Object to activate when interacting with the chest

        private bool canReach; // Flag to track if the player is within reach of the chest

        void Start()
        {
            // Initialize references and states
            chestObj = this.gameObject; // Assign the current GameObject (which this script is attached to) to OB
            handImg.SetActive(false); // Initially deactivate the hand UI element
            objInChest.SetActive(false); // Initially deactivate the object to activate
        }

        void OnTriggerEnter(Collider other)
        {
            // Triggered when another collider enters the trigger zone of this GameObject
            if (other.gameObject.tag == "Reach")
            {
                // Check if the entering collider has a tag "Reach"
                canReach = true; // Player is now in reach of the chest
                handImg.SetActive(true); // Activate the hand UI to indicate interaction
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Triggered when another collider exits the trigger zone of this GameObject
            if (other.gameObject.tag == "Reach")
            {
                // Check if the exiting collider has a tag "Reach"
                canReach = false; // Player is no longer in reach of the chest
                handImg.SetActive(false); // Deactivate the hand UI
            }
        }

        void Update()
        {
            // Update is called once per frame
            if (canReach && Input.GetButtonDown("Interact")) //when press E
            {
                // Check if the player is in reach and presses the interact button
                handImg.SetActive(false); // Deactivate the hand UI
                objInChest.SetActive(true); // Activate the designated object
                chestObj.GetComponent<Animator>().SetBool("open", true); // Set "open" parameter of the chest's Animator to true
                chestObj.GetComponent<BoxCollider>().enabled = false; // Disable the BoxCollider of the chest to prevent further interactions
            }
        }
    }
}
