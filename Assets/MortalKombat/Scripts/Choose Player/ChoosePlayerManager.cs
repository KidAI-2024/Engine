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

        // 2 public panels ui for player 1 and player 2
        public GameObject player1InfoPanel;
        public GameObject player2InfoPanel;

        public GameObject ninjaButton;
        public GameObject hulkButton;

        // 2 public game objects for player 1 and player 2
        public GameObject player1;
        public GameObject player2;

        public bool player1Ready = false;
        public bool player2Ready = false;

        string selectedPlayer1Name = "";
        string selectedPlayer2Name = "";

        void Start(){
            gameManager = GameManager.Instance;
            // add event listener to player 1 button
            player1Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player1Ready);
            // add event listener to player 2 button
            player2Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player2Ready);
            player1Text.text = "";
            player2Text.text = "";

            // initialize player 1 and player 2 info panel
            OnCharacterPlayerChange("Ninja");
            OnCharacterPlayerChange("Hulk");
            ninjaButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Ninja"));
            hulkButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Hulk"));
        }

        public void Player1Ready()
        {
            player1Ready = !player1Ready;
            if (player1Ready)
            {
                gameManager.player1Name = selectedPlayer1Name;
                player1Text.text = "Ready";
                player1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Unready";
            }
            else
            {
                player1Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Ready";
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
                gameManager.player2Name = selectedPlayer2Name;
                player2Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Unready";
                player2Text.text = "Ready";
            }
            else
            { 
                player2Button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Ready";
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


        void OnCharacterPlayerChange(string characterName)
        {   
            switch(characterName)
            {
                case "Ninja":
                    selectedPlayer1Name = "Ninja";
                    // Health
                    player1InfoPanel.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.7f;
                    // Power
                    player1InfoPanel.transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.6f;
                    // Speed
                    player1InfoPanel.transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.96f;
                    break;
                case "Hulk":
                    selectedPlayer2Name = "Hulk";
                    // Health
                    player2InfoPanel.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.85f;
                    // Power
                    player2InfoPanel.transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.78f;
                    // Speed
                    player2InfoPanel.transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = 0.5f;
                    break;
            }
        }

    }
}
