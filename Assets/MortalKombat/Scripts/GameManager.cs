using UnityEngine;

namespace MortalKombat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameObject ninjaPrefab;
        public GameObject hulkPrefab;

        GameObject player1;
        GameObject player2;
        
        public string player1Name;
        public string player2Name;
        public int Round = 1;
        public int RoundScore = 0;

        private ProjectController projectController;

        void Awake()
        {   
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            projectController = ProjectController.Instance;
            InverseClassToCtrlMapping();
        }
        void InverseClassToCtrlMapping()
        {
            foreach (var item in projectController.classesToControlsMap)
            {
                projectController.ControlsToclassesMap[item.Value] = item.Key;
                Debug.Log("Control: " + item.Value + " Class: " + item.Key);
            }
        }

        public void InstantiateCharacters()
        {
            if (player1Name == "Ninja")
            {
                player1 = Instantiate(ninjaPrefab);
                player1.name = "Player1";
                var player1Controller = player1.GetComponent<Player1Controller>();
                player1Controller.health = 100;
                player1Controller.primaryPower = 10;
                player1Controller.secondaryPower = 15;
                player1Controller.speed = 2.5f;
                // constants for  first player
                player1Controller.startLimit = 33.8f;
                player1Controller.endLimit = 28.3f;
                player1Controller.controls = SetPlayer1Controls();
            }



            if (player2Name == "Hulk")
            {
                player2 = Instantiate(hulkPrefab);
                player2.name = "Player2";
                var player2Controller = player2.GetComponent<Player1Controller>();
                player2Controller.health = 150;
                player2Controller.primaryPower = 20;
                player2Controller.secondaryPower = 15;
                player2Controller.speed = 1.5f;
                // constants for  second player
                player2Controller.startLimit = 28.3f;
                player2Controller.endLimit = 33.8f;
                player2Controller.controls = SetPlayer2Controls();
            }
        }

        Controls SetPlayer1Controls()
        {
            return new Controls
            {
                forward = "d",
                backward = "a",
                jump = "w",
                primaryHit = "e",
                secondaryHit = "space",
                block = "q",
                isEnabled = true
            };
        }
        Controls SetPlayer2Controls()
        {
            return new Controls
            {
                forward = "left",
                backward = "right",
                jump = "up",
                primaryHit = "/",
                secondaryHit = ".",
                block = "down",
                isEnabled = true
            };
        }
    }
}
