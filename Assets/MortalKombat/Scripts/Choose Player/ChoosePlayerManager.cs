using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat.ChoosePlayer
{

    public class ChoosePlayerManager : MonoBehaviour
    {
        public GameObject player1;
        public GameObject player2;
        public bool player1Ready = false;
        public bool player2Ready = false;

        public void Player1Ready()
        {
            player1Ready = !player1Ready;
            if (player1Ready)
            {
                string player1Name = player1.transform.GetChild(0).name;
                PlayerPrefs.SetString("Player1", player1Name);
                Debug.Log(player1Name);
            }
            CheckBothPlayersReady();
        }
        public void Player2Ready()
        {
            player2Ready = !player2Ready;
            if (player2Ready)
            {
                string player2Name = player2.transform.GetChild(0).name;
                PlayerPrefs.SetString("Player2", player2Name);
                Debug.Log(player2Name);
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

    }
}
