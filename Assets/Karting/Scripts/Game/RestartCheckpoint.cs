using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karting.Game
{
    public class RestartCheckpoint : MonoBehaviour
    {
        public void RestartCheckpoints()
        {
            // Get all children of the parent object
            Transform[] checkpoints = GetComponentsInChildren<Transform>(true);
            // Loop through all the children
            foreach (Transform checkpoint in checkpoints)
            {
                // Check if the child has the tag "KartingCheckpoint"
                if (checkpoint.CompareTag("KartingCheckpoint"))
                {
                    // Set the checkpoint to active
                    // Debug.Log("Checkpoint: " + checkpoint.name);
                    checkpoint.gameObject.SetActive(true);
                }
            }
        }
    }
}