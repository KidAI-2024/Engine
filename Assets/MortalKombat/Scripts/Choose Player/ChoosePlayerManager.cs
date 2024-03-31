using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat.ChoosePlayer
{

    public class ChoosePlayerManager : MonoBehaviour
    {
        public static GameManager gameManager;
        // 2 public buttons for player 1 and player 2
        public GameObject player1Button;
        public GameObject player2Button;

        // 2 public text mesh pro texts for player 1 and player 2
        public TMPro.TextMeshProUGUI player1Text;
        public TMPro.TextMeshProUGUI player2Text;

        // 2 public game objects for player 1 and player 2
        public GameObject player1;
        public GameObject player2;

        public bool player1Ready = false;
        public bool player2Ready = false;


        void Start(){
            gameManager = GameManager.Instance;
            // add event listener to player 1 button
            player1Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player1Ready);
            // add event listener to player 2 button
            player2Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player2Ready);
            player1Text.text = "";
            player2Text.text = "";
        }

        public void Player1Ready()
        {
            player1Ready = !player1Ready;
            if (player1Ready)
            {
                string player1Name = player1.transform.GetChild(0).name;
                gameManager.player1Name = player1Name;
                player1Text.text = "Ready";
                player1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Unready";
                Debug.Log(player1Name);
            }
            else
            {
                gameManager.player1Name = "";
                player1Text.text = "";
            }
            CheckBothPlayersReady();
        }
        public void Player2Ready()
        {
            player2Ready = !player2Ready;
            if (player2Ready)
            {
                string player2Name = player2.transform.GetChild(0).name;
                gameManager.player2Name = player2Name;
                player1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Unready";
                player2Text.text = "Ready";
                Debug.Log(player2Name);
            }
            else
            { 
                gameManager.player2Name = "";
                player2Text.text = "";
            }
            CheckBothPlayersReady();
        }
        void CheckBothPlayersReady()
        {
            if(player1Ready && player2Ready)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Mortal Kombat");
            }
        }

    }
}
