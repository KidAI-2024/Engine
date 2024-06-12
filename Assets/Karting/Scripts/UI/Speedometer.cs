using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace Karting.UI
{

    public class Speedometer : MonoBehaviour
    {
        public Rigidbody carRigidbody;
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI gearText;
        public Slider speedSlider;

        private void Update()
        {
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
