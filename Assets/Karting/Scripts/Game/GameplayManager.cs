using UnityEngine;
namespace Karting.Game
{

    public class GameplayManager : MonoBehaviour
    {
        public Transform carSpawnPoint;

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

            instantiatedCar.name = "PlayerCar";
        }
    }
}
