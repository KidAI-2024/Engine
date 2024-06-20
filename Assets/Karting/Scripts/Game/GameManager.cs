using UnityEngine;
using UnityEngine.SceneManagement;
namespace Karting.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        // Store selected data
        public string selectedGameMode = "Timed";
        public string selectedRaceTrack = "KartingTrack2";
        public GameObject selectedCarPrefab;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void SelectGameMode(string mode)
        {
            selectedRaceTrack = mode;
            Debug.Log("Game mode selected: " + selectedRaceTrack);
        }


        public void SelectRaceTrack(string sceneName)
        {
            selectedRaceTrack = sceneName;
            Debug.Log("Track selected: " + selectedRaceTrack);
        }

        public void SelectCar(GameObject carPrefab)
        {
            selectedCarPrefab = carPrefab;
            Debug.Log("Car selected: " + selectedCarPrefab.name);
        }

        // Call this method to start the race
        public void StartCustomMatch()
        {
            SceneManager.LoadScene(selectedRaceTrack.ToString());

        }
        public void StartQuickMatch()
        {
            SelectGameMode("Timed");
            SelectRaceTrack("KartingTrack2");
            StartCustomMatch();
        }
    }

    // Enum for different game modes
}
