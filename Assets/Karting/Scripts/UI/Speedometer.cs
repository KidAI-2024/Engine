using UnityEngine;
using TMPro;
namespace Karting.UI
{

    public class Speedometer : MonoBehaviour
    {
        public Rigidbody carRigidbody;
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI gearText;

        private void Update()
        {
            // get speed and convert to km/h
            float speed = carRigidbody.velocity.magnitude * 3.6f;

            // check if the car is moving forward
            bool isMovingForward = Vector3.Dot(carRigidbody.velocity, carRigidbody.transform.forward) >= 0;

            // Update speedometer display
            speedText.text = Mathf.RoundToInt(speed).ToString();

            // Update direction display
            gearText.text = isMovingForward ? "D" : "R";
        }
    }
}
