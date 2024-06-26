using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MortalKombat
{
    public class SelectMap : MonoBehaviour
    {
        public GameObject MapsPanel;
        public GameObject MapNameText;
        public Sprite map1_image;
        public Sprite map2_image;
        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.mapName = "Forest";
        }
        public void LoadSelectedMap()
        {
            // load the selected map name
            SceneManager.LoadScene(gameManager.mapName);
        }
        public void NextMap()
        {
            gameManager.mapName = "Forest";
            MapsPanel.GetComponent<Image>().sprite = map1_image;
            MapNameText.GetComponent<TextMeshProUGUI>().text = "Forest";
            // uncomment nex lines when added new map
            // if (gameManager.mapName == "Forest")
            // {
            //     gameManager.mapName = "Desert";
            //     MapsPanel.GetComponent<Image>().sprite = map2_image;
            //     MapNameText.GetComponent<TextMeshProUGUI>().text = "Desert";
            // }
            // else
            // {
            //     gameManager.mapName = "Forest";
            //     MapsPanel.GetComponent<Image>().sprite = map1_image;
            //     MapNameText.GetComponent<TextMeshProUGUI>().text = "Forest";
            // }
        }
    }
}

