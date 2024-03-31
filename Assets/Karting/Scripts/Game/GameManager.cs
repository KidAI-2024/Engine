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
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        public void SelectGameMode_Timed()
        {
            selectedGameMode = GameMode.Timed;
        }

        public void SelectRaceTrack_Forest()
        {
            selectedRaceTrack = RaceTrack.KartingTrack2;
        }
        public void SelectCar_Dodge()
        {
            SelectCar("Car_02");
        }
        void SelectCar(string carName)
        {
            // Load car prefab using its name
            selectedCarPrefab = Resources.Load<GameObject>("../../Prefabs/" + carName);
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
            SelectCar_Dodge();
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
