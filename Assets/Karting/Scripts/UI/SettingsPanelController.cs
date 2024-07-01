using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Karting.UI
{

    public class SettingsPanelController : MonoBehaviour
    {
        public GameObject settingsPanel;
        public Button settingsButton;
        public Button exitButton;
        public Button returnToLobbyButton;
        bool isSettingsPanelActive = false;
        void Start()
        {
            // Hide the settings panel initially
            if (settingsPanel != null)
            {

                settingsPanel.SetActive(false);
            }
            else
            {
                Debug.Log("Settings Panel is null in SettingsPanelController");
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(ToggleSettingsPanel);
            }
            else
            {
                Debug.Log("Settings Button is null in SettingsPanelController");
            }
            if (exitButton != null)
            {
                // change scene to kartingintro
                // exitButton.onClick.AddListener(() => SceneManager.LoadScene("KartingIntro"));
                // quit app
                exitButton.onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
            }
            else
            {
                Debug.Log("Exit Button is null in SettingsPanelController");
            }
            if (returnToLobbyButton != null)
            {
                // change scene to lobby
                returnToLobbyButton.onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
            }
            else
            {
                Debug.Log("Return To Lobby Button is null in SettingsPanelController");
            }
        }
        // This function will be called when the button is clicked
        void Quit()
        {
            // Log a message in the console
            Debug.Log("Application is quitting");
            
            // If we are running in a standalone build of the game
#if UNITY_STANDALONE
            // Quit the application
            Application.Quit();
#endif

            // If we are running in the editor
#if UNITY_EDITOR
            // Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        void Update()
        {
            // Toggle settings panel when the Escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsPanel();
            }
        }

        public void ToggleSettingsPanel()
        {
            settingsPanel.SetActive(!isSettingsPanelActive);
            isSettingsPanelActive = !isSettingsPanelActive;
        }
        public void CloseSettingsPanel()
        {
            settingsPanel.SetActive(false);
            isSettingsPanelActive = false;
        }

    }
}
