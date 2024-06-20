using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace Karting.UI
{

    public class Speedometer : MonoBehaviour
    {
        Rigidbody carRigidbody;
        GameObject attachedVehicle;

        public TextMeshProUGUI speedText;
        public TextMeshProUGUI gearText;
        public Slider speedSlider;

        void start()
        {
            attachedVehicle = GameObject.Find("PlayerCar");
            if (attachedVehicle != null)
            {
                carRigidbody = attachedVehicle.GetComponent<Rigidbody>();
                if (carRigidbody == null)
                {
                    Debug.Log("Car Rigidbody not found");
                }
            }
            else
            {
                Debug.Log("Car not found");
            }
        }

        private void Update()
        {
            if (carRigidbody == null)
            {
                attachedVehicle = GameObject.Find("PlayerCar");
                if (attachedVehicle != null)
                {
                    carRigidbody = attachedVehicle.GetComponent<Rigidbody>();
                    if (carRigidbody == null)
                    {
                        Debug.Log("Car Rigidbody not found");
                    }
                }
                else
                {
                    Debug.Log("Car not found");
                }
            }
            // get speed and convert to km/h
            float speed = carRigidbody.velocity.magnitude * 3.6f;

            // check if the car is moving forward
            bool isMovingForward = Vector3.Dot(carRigidbody.velocity, carRigidbody.transform.forward) >= 0;

            // Update speedometer display
            speedText.text = Mathf.RoundToInt(speed).ToString();
            speedSlider.value = speed / 220;

            // Update direction display
            gearText.text = isMovingForward ? "D" : "R";
            gearText.text = speed > 1 ? gearText.text : "N";
        }
        void ondestroy()
        {
            carRigidbody = null;
        }
    }
}
