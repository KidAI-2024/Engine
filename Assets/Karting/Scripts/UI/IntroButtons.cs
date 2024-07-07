using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Karting.UI
{

    public class IntroButtons : MonoBehaviour
    {
        public Button quickMatchButton;
        public Button customMatchButton;
        public GlobalAssets.UI.LoadingPanelController loadingPanelContoller;
        // Start is called before the first frame update
        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("Game Manager is null in IntroButtons");
            }
            if (quickMatchButton != null)
            {
                quickMatchButton.onClick.AddListener(() => {
                    loadingPanelContoller.ShowLoadingPanel();
                    gameManager.StartQuickMatch(); 
                });
            }
            else
            {
                Debug.LogError("Quick Match Button is null in IntroButtons");
            }
            if (customMatchButton != null)
            {
                customMatchButton.onClick.AddListener(() => {
                    loadingPanelContoller.ShowLoadingPanel();
                    SceneManager.LoadScene("KartingChooseTrack");
                });
            }
            else
            {
                Debug.LogError("Custom Match Button is null in IntroButtons");
            }

        }

        void ondestroy()
        {
            quickMatchButton.onClick.RemoveAllListeners();
            customMatchButton.onClick.RemoveAllListeners();
        }
    }
}
