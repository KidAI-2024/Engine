using UnityEngine;
namespace Karting.Game
{

    public class GameplayManager : MonoBehaviour
    {
        public Transform carSpawnPoint;
        public GameObject cameraController;
        public GameObject speedometer;

        private GameObject instantiatedCar;


        // Start is called before the first frame update
        void Awake()
        {
            if (GameManager.instance.selectedCarPrefab == null)
            {
                Debug.LogError("No car selected in GameManager");
                return;
            }
            // Instantiate the selected car prefab
            instantiatedCar = Instantiate(GameManager.instance.selectedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation);

            // Attach car to camera controller if it exists
            if (cameraController != null)
            {
                Camera.CameraFollow cameraFollow = cameraController.GetComponent<Camera.CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.atachedVehicle = instantiatedCar;
                }
                else
                {
                    Debug.LogWarning("Camera controller does not have CameraFollow component");
                }
            }
            else
            {
                Debug.LogWarning("Camera controller not found");
            }
            // Attach car to speedometer if it exists
            if (speedometer != null)
            {
                UI.Speedometer speedometerScript = speedometer.GetComponent<UI.Speedometer>();
                if (speedometerScript != null)
                {
                    speedometerScript.carRigidbody = instantiatedCar.GetComponent<Rigidbody>();
                }
                else
                {
                    Debug.LogWarning("Speedometer does not have Speedometer component");
                }
            }
            else
            {
                Debug.LogWarning("Speedometer not found");
            }
        }
    }
}
