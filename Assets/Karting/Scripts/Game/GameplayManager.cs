using UnityEngine;
namespace Karting.Game
{
    public class GameplayManager : MonoBehaviour
    {
        public Transform carSpawnPoint;

        public float time { get; set; } = 0.0f;
        public GameObject checkpoints;
        public GameObject hiddenCheckpoints;

        private GameObject instantiatedCar;
        private Karting.Game.RestartCheckpoint restartCheckpoint;
        private Karting.Game.RestartCheckpoint restartHiddenCheckpoint;

        public bool win { get; private set; } = false;
        private Karting.Car.CarController3 carController;

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

            instantiatedCar.name = "PlayerCar";
            // Get the RestartCheckpoint component
            restartCheckpoint = checkpoints.GetComponent<Karting.Game.RestartCheckpoint>();
            if (restartCheckpoint == null)
            {
                Debug.LogError("No RestartCheckpoint component found in GameplayManager");
            }
            // Get the RestartHiddenCheckpoint component
            restartHiddenCheckpoint = hiddenCheckpoints.GetComponent<Karting.Game.RestartCheckpoint>();
            if (restartHiddenCheckpoint == null)
            {
                Debug.LogError("No RestartHiddenCheckpoint component found in GameplayManager");
            }
            carController = instantiatedCar.GetComponent<Karting.Car.CarController3>();
            if (carController == null)
            {
                Debug.LogError("No CarController3 component found in GameplayManager");
            }
        }
        public void RestartGame()
        {
            ResetCarToSpawnPoint();
            if (restartCheckpoint != null)
            {
                restartCheckpoint.RestartCheckpoints();
                restartHiddenCheckpoint.RestartCheckpoints();
            }
            else
            {
                Debug.LogError("No RestartCheckpoint component found in GameplayManager");
            }
            time = 0.0f;
            carController.SetCanMove(true);
            win = false;
        }
        public void ResetCarToSpawnPoint()
        {
            // set speed to 0
            instantiatedCar.GetComponent<Rigidbody>().velocity = Vector3.zero;
            instantiatedCar.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            // reset car position to spawn point
            instantiatedCar.transform.position = carSpawnPoint.position;
            instantiatedCar.transform.rotation = carSpawnPoint.rotation;
        }
        public void Win()
        {
            win = true;
            //set velocity to zero gradually using lerp
            //instantiatedCar.GetComponent<Rigidbody>().velocity = Vector3.Lerp(instantiatedCar.GetComponent<Rigidbody>().velocity, Vector3.zero, 0.1f);
            //set angular velocity to zero gradually using lerp
            //instantiatedCar.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(instantiatedCar.GetComponent<Rigidbody>().angularVelocity, Vector3.zero, 0.1f);

            //carController.SetCanMove(false);

        }
    }
}
