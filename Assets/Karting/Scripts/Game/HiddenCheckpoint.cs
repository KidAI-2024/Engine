using Karting.Car;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Karting.Car.CarController3;
namespace Karting.Game
{
    public class HiddenCheckpoint : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            // Debug.Log("Collided with " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hidden Checkpoint reached");
                // set active false
                gameObject.SetActive(false);
            }
        }
    }
}
