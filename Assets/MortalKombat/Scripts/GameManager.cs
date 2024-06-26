using UnityEngine;
using System.Collections.Generic;

namespace MortalKombat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameObject ninjaPrefab;
        public GameObject archerPrefab;
        public GameObject cannonPrefab;
        
        public GameObject hulkPrefab;
        public GameObject cryptoPrefab;
        public GameObject zombiePrefab;
        
        public string mapName = "Forest";
        public string player1Name;
        public string player2Name;
        public int Round = 1;
        public int RoundScore = 0;
        public int Player1ScoreValue = 0;
        public int Player2ScoreValue = 0;
        
        public float volume = 1f;
        public bool mute = false;
        public bool predictIsOn = true;

        public GameObject player1;
        public GameObject player2;
        ProjectController projectController;

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
                // Debug.Log("Control: " + item.Value + " Class: " + item.Key);
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
            else if (player1Name == "Archer")
            {
                player1 = Instantiate(archerPrefab);
                player1.name = "Player1";
                var player1Controller = player1.GetComponent<Player1Controller>();
                player1Controller.health = 120;
                player1Controller.primaryPower = 15;
                player1Controller.secondaryPower = 10;
                player1Controller.speed = 2.0f;
                // constants for  first player
                player1Controller.startLimit = 33.8f;
                player1Controller.endLimit = 28.3f;
                player1Controller.controls = SetPlayer1Controls();
            }
            else if (player1Name == "Cannon")
            {
                player1 = Instantiate(cannonPrefab);
                player1.name = "Player1";
                var player1Controller = player1.GetComponent<Player1Controller>();
                player1Controller.health = 155;
                player1Controller.primaryPower = 25;
                player1Controller.secondaryPower = 5;
                player1Controller.speed = 1.3f;
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
            else if (player2Name == "Crypto")
            {
                player2 = Instantiate(cryptoPrefab);
                player2.name = "Player2";
                var player2Controller = player2.GetComponent<Player1Controller>();
                player2Controller.health = 110;
                player2Controller.primaryPower = 12;
                player2Controller.secondaryPower = 10;
                player2Controller.speed = 2.3f;
                // constants for  second player
                player2Controller.startLimit = 28.3f;
                player2Controller.endLimit = 33.8f;
                player2Controller.controls = SetPlayer2Controls();
            }
            else if (player2Name == "Zombie")
            {
                player2 = Instantiate(zombiePrefab);
                player2.name = "Player2";
                var player2Controller = player2.GetComponent<Player1Controller>();
                player2Controller.health = 90;
                player2Controller.primaryPower = 25;
                player2Controller.secondaryPower = 20;
                player2Controller.speed = 1.8f;
                // constants for  second player
                player2Controller.startLimit = 28.3f;
                player2Controller.endLimit = 33.8f;
                player2Controller.controls = SetPlayer2Controls();
            }

            // if(player1 != null) 
            // {
            //     if(mapName == "Forest")
            //     {
            //         // set player1 position to (20.11,0.19,33.5) and rotation to (0,180,0)
            //         player1.transform.position = new Vector3(20.11f, 0.19f, 33.5f);
            //         player1.transform.rotation = Quaternion.Euler(0, 180, 0);
            //     }
            // }
            // if(player2 != null)
            // {
            //     if(mapName == "Forest")
            //     {
            //         // set player2 position to (20.11,0.2,28.9) and rotation to (0,0,0)
            //         player2.transform.position = new Vector3(20.11f, 0.2f, 28.9f);
            //         player2.transform.rotation = Quaternion.Euler(0, 0, 0);
            //     }
            // }
        }

        Controls SetPlayer1Controls()
        {
            return new Controls
            {
                forward = new List<string>{"d", projectController.ControlsToclassesMap["Right"]},
                backward = new List<string>{"a", projectController.ControlsToclassesMap["Left"]},
                jump = "w", 
                primaryHit = new List<string>{"e", projectController.ControlsToclassesMap["Primary"]},
                secondaryHit = new List<string>{"space", projectController.ControlsToclassesMap["Secondary"]},
                block = "q",
                isEnabled = true
            };
        }
        Controls SetPlayer2Controls()
        {
            return new Controls
            {
                forward = new List<string>{"left","_"},
                backward = new List<string>{"right","_"},
                jump = "up",
                primaryHit = new List<string>{"/","_"},
                secondaryHit = new List<string>{".","_"},
                block = "down",
                isEnabled = true
            };
        }
        public void Reset()
        {
            Round = 1;
            RoundScore = 0;
            Player1ScoreValue = 0;
            Player2ScoreValue = 0;
        }

        public void RestartGame()
        {
            Reset();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
