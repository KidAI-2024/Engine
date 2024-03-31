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
        }
        public void InstantiateCharacters()
        {
            Debug.Log("Instantiating Characters");
            if (player1Name == "Ninja")
            {
                player1 = Instantiate(ninjaPrefab);
                player1.name = "Player1";
            }
            else{
                Debug.Log("Player 1 not instantiated");
            }



            if (player2Name == "Hulk")
            {
                player2 = Instantiate(hulkPrefab);
                player2.name = "Player2";
            }else{
                Debug.Log("Player 2 not instantiated");
            }

        }
    }
}
