using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Karting.Game
{
    public class Checkpoint : MonoBehaviour
    {
        Karting.Car.CarController3 carController;
        Karting.Car.CarController3.StatPowerup statPowerup;
        // Start is called before the first frame update
        void Start()
        {
            statPowerup = new Karting.Car.CarController3.StatPowerup();
            statPowerup.modifiers.TopSpeed = 5f;
            statPowerup.modifiers.Acceleration = 1.5f;
            statPowerup.ElapsedTime = 0.0f;
            statPowerup.MaxTime = 3.0f;
            statPowerup.PowerUpID = gameObject.name;

        }
        // OnTriggerEnter is called when the Collider other enters the trigger
        void OnTriggerEnter(Collider other)
        {
            // Debug.Log("Collided with " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Checkpoint reached");
                carController = FindObjectOfType<Karting.Car.CarController3>();
                carController.AddPowerup(statPowerup);
                // set active false
                gameObject.SetActive(false);
            }
        }
    }
}
