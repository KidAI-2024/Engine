using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MortalKombat 
{
    public class Settings : MonoBehaviour
    {
        public static Settings Instance { get; private set; }

        public GameObject Camera;
        public GameObject AudioSources;

        public GameObject CameraToggleButton;
        public GameObject MuteToggleButton;
        public GameObject VolumeSlider;

        private GameObject player1;
        private GameObject player2;

        private GameManager gameManager;
        void Start()
        {
            gameManager = GameManager.Instance;  
            
            if(gameManager.mute)
            {
                MuteToggleButton.GetComponent<UnityEngine.UI.Toggle>().isOn = true;
                ToggleMute();
            }
            Volume(gameManager.volume);

            // add listener to the mute toggle button
            MuteToggleButton.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener(delegate {
                ToggleMute();
            });
            
            // add listener to the volume slider
            VolumeSlider.GetComponent<UnityEngine.UI.Slider>().onValueChanged.AddListener(delegate {
                Volume(VolumeSlider.GetComponent<UnityEngine.UI.Slider>().value);
            });
        }
        public void ToggleMute()
        {
            gameManager.mute = MuteToggleButton.GetComponent<UnityEngine.UI.Toggle>().isOn;
            if (gameManager.mute)
            {
                VolumeSlider.GetComponent<UnityEngine.UI.Slider>().value = 0;
                Volume(0);
            }
            else
            {
                if (gameManager.volume == 0)
                {
                    VolumeSlider.GetComponent<UnityEngine.UI.Slider>().value = 1;
                    Volume(1);
                }
                // Unmute all audio sources
                for (int i = 0; i < AudioSources.transform.childCount; i++)
                {
                    AudioSources.transform.GetChild(i).GetComponent<AudioSource>().mute = false;
                }
            }
        }
        public void Volume(float volume)
        {
            gameManager.volume = volume;

            for (int i = 0; i < AudioSources.transform.childCount; i++)
            {
                AudioSources.transform.GetChild(i).GetComponent<AudioSource>().volume = volume;
            }
            // player1.GetComponent<AudioSource>().volume = volume;
            // player2.GetComponent<AudioSource>().volume = volume;

            // if the volume is 0, set the mute toggle button to true
            if (volume == 0)
            {
                MuteToggleButton.GetComponent<UnityEngine.UI.Toggle>().isOn = true;
            }
            else
            {
                MuteToggleButton.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
                ToggleMute();
            }
        }
        public void ToggleCamera()
        {
            Camera.SetActive(!Camera.activeSelf);
        }

    }
}
