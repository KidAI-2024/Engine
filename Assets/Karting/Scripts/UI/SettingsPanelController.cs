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
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

    }
}
