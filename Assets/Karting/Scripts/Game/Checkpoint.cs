using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Karting.Game
{
    public class Checkpoint : MonoBehaviour
    {
        // OnTriggerEnter is called when the Collider other enters the trigger
        void OnTriggerEnter(Collider other)
        {
            // Debug.Log("Collided with " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Checkpoint reached");
                // set active false
                gameObject.SetActive(false);
            }
        }
    }
}
