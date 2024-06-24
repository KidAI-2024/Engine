using UnityEngine;
namespace Karting.Game
{
    public class GameplayManager : MonoBehaviour
    {
        public Transform carSpawnPoint;

        public float time { get; set; } = 0.0f;
        public GameObject checkpoints;

        private GameObject instantiatedCar;
        private Karting.Game.RestartCheckpoint restartCheckpoint;

        public bool win { get; private set; } = false;


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
        }
        public void RestartGame()
        {
            ResetCarToSpawnPoint();
            if (restartCheckpoint != null)
            {
                restartCheckpoint.RestartCheckpoints();
            }
            else
            {
                Debug.LogError("No RestartCheckpoint component found in GameplayManager");
            }
            time = 0.0f;
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

        }
    }
}
