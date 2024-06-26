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
        public GameObject archerButton;
        public GameObject cannonButton;

        public GameObject hulkButton;
        public GameObject cryptoButton;
        public GameObject zombieButton;

        // 2 public game objects for player 1 and player 2
        public GameObject player1;
        public GameObject player2;

        public bool player1Ready = false;
        public bool player2Ready = false;

        string selectedPlayer1Name = "";
        string selectedPlayer2Name = "";
        
        private AudioSource audioSource;
        public AudioClip ninja_sfx;
        public AudioClip hulk_sfx;
        public AudioClip cannon_sfx;
        public AudioClip archer_sfx;
        public AudioClip crypto_sfx;
        public AudioClip zombie_sfx;
        
        void Start(){
            gameManager = GameManager.Instance;
            // add event listener to player 1 button
            player1Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player1Ready);
            // add event listener to player 2 button
            player2Button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Player2Ready);
            player1Text.text = "";
            player2Text.text = "";

            // initialize player 1 and player 2 info panel
            OnCharacterPlayerChange("Ninja",true);
            OnCharacterPlayerChange("Hulk",true);

            ninjaButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Ninja"));
            archerButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Archer"));
            cannonButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Cannon"));
            hulkButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Hulk"));
            cryptoButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Crypto"));
            zombieButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnCharacterPlayerChange("Zombie"));
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
                // lock the 2 buttons and change the scene after 3 seconds
                player1Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
                player2Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
                StartCoroutine(LoadChooseMapScene());
            }
        }
        IEnumerator LoadChooseMapScene()
        {
            yield return new WaitForSeconds(1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectMap");
        }

        void OnCharacterPlayerChange(string characterName, bool isFirstPlayer = false)
        {   
            switch(characterName)
            {
                case "Ninja":
                    player1.transform.GetChild(0).gameObject.SetActive(true);
                    player1.transform.GetChild(1).gameObject.SetActive(false);
                    player1.transform.GetChild(2).gameObject.SetActive(false);
                    ninjaButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    archerButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    cannonButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    if(!isFirstPlayer)
                    {
                        audioSource = ninjaButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(ninja_sfx);
                    }
                    selectedPlayer1Name = "Ninja";
                    ChoosePlayer(player1InfoPanel, "NINJA", 0.7f, 0.6f, 0.96f);
                    break;
                case "Archer":
                    player1.transform.GetChild(0).gameObject.SetActive(false);
                    player1.transform.GetChild(1).gameObject.SetActive(true);
                    player1.transform.GetChild(2).gameObject.SetActive(false);
                    archerButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    ninjaButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    cannonButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    if(!isFirstPlayer)
                    {
                        audioSource = archerButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(archer_sfx);
                    }
                    selectedPlayer1Name = "Archer";
                    ChoosePlayer(player1InfoPanel, "ARCHER", 0.8f, 0.7f, 0.75f);
                    break;
                case "Cannon":
                    player1.transform.GetChild(0).gameObject.SetActive(false);
                    player1.transform.GetChild(1).gameObject.SetActive(false);
                    player1.transform.GetChild(2).gameObject.SetActive(true);
                    cannonButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    ninjaButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    archerButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    if(!isFirstPlayer)
                    {
                        audioSource = cannonButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(cannon_sfx);
                    }
                    selectedPlayer1Name = "Cannon";
                    ChoosePlayer(player1InfoPanel, "CANNON", 0.9f, 0.8f, 0.45f);
                    break;
                case "Hulk":
                    player2.transform.GetChild(0).gameObject.SetActive(true);
                    player2.transform.GetChild(1).gameObject.SetActive(false);
                    player2.transform.GetChild(2).gameObject.SetActive(false);
                    hulkButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    cryptoButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    if(!isFirstPlayer)
                    {
                        audioSource = hulkButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(hulk_sfx);
                    }
                    selectedPlayer2Name = "Hulk";
                    ChoosePlayer(player2InfoPanel, "HULK", 0.85f, 0.78f, 0.5f);
                    break;
                case "Crypto":
                    player2.transform.GetChild(0).gameObject.SetActive(false);
                    player2.transform.GetChild(1).gameObject.SetActive(true);
                    player2.transform.GetChild(2).gameObject.SetActive(false);
                    cryptoButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    hulkButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();

                    if(!isFirstPlayer)
                    {
                        audioSource = cryptoButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(crypto_sfx);
                    }
                    selectedPlayer2Name = "Crypto";
                    ChoosePlayer(player2InfoPanel, "CRYPTO", 0.75f, 0.65f, 0.9f);
                    break;

                case "Zombie":
                    player2.transform.GetChild(0).gameObject.SetActive(false);
                    player2.transform.GetChild(1).gameObject.SetActive(false);
                    player2.transform.GetChild(2).gameObject.SetActive(true);
                    zombieButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonSelected();
                    cryptoButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    if(!isFirstPlayer)
                    {
                        audioSource = zombieButton.GetComponent<UnityEngine.UI.Button>().GetComponent<AudioSource>();
                        audioSource.PlayOneShot(zombie_sfx);
                    }
                    selectedPlayer2Name = "Zombie";
                    ChoosePlayer(player2InfoPanel, "ZOMBIE", 0.65f, 0.75f, 0.6f);
                    break;
                default:
                    archerButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    ninjaButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    hulkButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    cryptoButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    cannonButton.GetComponent<UnityEngine.UI.Button>().GetComponent<ButtonHoverController>().ButtonDeselected();
                    break;
            }
        }
        void ChoosePlayer(GameObject playerPanel, string charName, float health, float power, float speed)
        {
            playerPanel.transform.parent.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = charName;
            playerPanel.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = health;
            playerPanel.transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = power;
            playerPanel.transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Slider>().value = speed;
        }
    }
}
