using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Karting.UI
{

    public class EndgamePanelController : MonoBehaviour
    {
        public GameObject endgamePanel;
        // Start is called before the first frame update
        void Start()
        {
            // Hide the endgame panel initially
            if (endgamePanel != null)
            {
                endgamePanel.SetActive(false);
            }
            else
            {
                Debug.Log("Endgame Panel is null in EndgamePanelController");
            }

        }

        public void ShowEndgamePanel()
        {
            endgamePanel.SetActive(true);
        }
    }

}