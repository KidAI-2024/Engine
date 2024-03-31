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
        public Button backButton;
        public Button exitButton;
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
            if (backButton != null)
            {

                backButton.onClick.AddListener(ToggleSettingsPanel);
            }
            else
            {
                Debug.Log("Back Button is null in SettingsPanelController");
            }
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(() => SceneManager.LoadScene("KartingIntro"));
            }
            else
            {
                Debug.Log("Exit Button is null in SettingsPanelController");
            }
        }

        void Update()
        {
            // Toggle settings panel when the Escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsPanel();
            }
        }

        void ToggleSettingsPanel()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
        void ondestroy()
        {
            backButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }
    }
}
