using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MortalKombat
{
    public class EnemyController : MonoBehaviour
    {
        private GameObject enemyPlayer;
        // Start is called before the first frame update
        void Start()
        {
            enemyPlayer = GameObject.Find("Player2");
        }

        // Update is called once per frame
        void Update()
        {
            // every 1 seconds, the enemy will randomly choose to move forward, backward, hit primary, hit secondary
            if (Time.time % 1 == 0)
            {
                int randomAction = Random.Range(0, 4);
                Debug.Log("Random Action: " + randomAction);
                switch (randomAction)
                {
                    case 0:
                        enemyPlayer.GetComponent<Player1Controller>().forwardAuto = true;
                        enemyPlayer.GetComponent<Player1Controller>().backwardAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().primaryHitAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().secondaryHitAuto = false;
                        break;
                    case 1:
                        enemyPlayer.GetComponent<Player1Controller>().backwardAuto = true;
                        enemyPlayer.GetComponent<Player1Controller>().forwardAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().primaryHitAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().secondaryHitAuto = false;
                        break;
                    case 2:
                        enemyPlayer.GetComponent<Player1Controller>().primaryHitAuto = true;
                        enemyPlayer.GetComponent<Player1Controller>().secondaryHitAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().forwardAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().backwardAuto = false;
                        break;
                    case 3:
                        enemyPlayer.GetComponent<Player1Controller>().secondaryHitAuto = true;
                        enemyPlayer.GetComponent<Player1Controller>().primaryHitAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().forwardAuto = false;
                        enemyPlayer.GetComponent<Player1Controller>().backwardAuto = false;
                        break;
                }
            }
        }
    }
}
