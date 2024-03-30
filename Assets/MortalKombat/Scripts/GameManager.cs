using UnityEngine;

namespace MortalKombat
{
    public class GameManager : MonoBehaviour
    {
        public GameObject player1Prefab;
        public GameObject player2Prefab;
        
        void Awake()
        {   
            string player1Name = PlayerPrefs.GetString("Player1");
            string player2Name = PlayerPrefs.GetString("Player2");
            if (player1Name == "Ninja")
            {
                GameObject player1 = Instantiate(player1Prefab);
                player1.name = "Player1";
            }
            if (player2Name == "Hulk")
            {
                GameObject player2 = Instantiate(player2Prefab);
                player2.name = "Player2";
            }
        }
    }
}
