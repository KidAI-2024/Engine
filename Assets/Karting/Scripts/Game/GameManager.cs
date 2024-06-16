using UnityEngine;
using UnityEngine.SceneManagement;
namespace Karting.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        // Store selected data
        public GameMode selectedGameMode;
        public RaceTrack selectedRaceTrack;
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

        public void SelectGameMode_Timed()
        {
            selectedGameMode = GameMode.Timed;
            Debug.Log("Game mode selected: " + selectedGameMode);
        }

        public void SelectRaceTrack_Forest()
        {
            selectedRaceTrack = RaceTrack.KartingTrack2;
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
            SelectGameMode_Timed();
            SelectRaceTrack_Forest();
            StartCustomMatch();
        }
    }

    // Enum for different game modes
    public enum GameMode
    {
        Timed
    }

    // Enum for different race tracks
    public enum RaceTrack
    {
        KartingTrack2
    }
}
