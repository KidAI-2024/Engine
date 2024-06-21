using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Karting.UI
{

    public class ChooseTrack : MonoBehaviour
    {
        public String nextSceneName = "KartingChooseCar";
        public List<TrackInfo> trackInfos;
        private int currentTrackIndex = 0;
        public Button selectButton;
        public Button nextButton;
        public Button previousButton;
        public Image trackImage;
        public Image trackMap;
        public TMPro.TextMeshProUGUI trackNameText;
        public TMPro.TextMeshProUGUI trackModeText;

        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            selectButton.onClick.AddListener(() => gameManager.SelectRaceTrack(trackInfos[currentTrackIndex].sceneName));
            selectButton.onClick.AddListener(() => SceneManager.LoadScene(nextSceneName));

            nextButton.onClick.AddListener(nextTrack);
            previousButton.onClick.AddListener(previousTrack);
            UpdateUI();

        }
        void UpdateUI()
        {
            trackImage.sprite = trackInfos[currentTrackIndex].image;
            trackMap.sprite = trackInfos[currentTrackIndex].map;
            trackNameText.text = trackInfos[currentTrackIndex].name;
            trackModeText.text = trackInfos[currentTrackIndex].mode;
        }
        void ondestroy()
        {
            selectButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            previousButton.onClick.RemoveAllListeners();
        }
        void nextTrack()
        {
            currentTrackIndex++;
            if (currentTrackIndex >= trackInfos.Count)
            {
                currentTrackIndex = 0;
            }
            UpdateUI();
        }
        void previousTrack()
        {
            currentTrackIndex--;
            if (currentTrackIndex < 0)
            {
                currentTrackIndex = trackInfos.Count - 1;
            }
            UpdateUI();
        }
        [Serializable]
        public struct TrackInfo
        {
            public string name;
            public string sceneName;
            public string mode;
            public Sprite image;
            public Sprite map;
        }
    }
}
