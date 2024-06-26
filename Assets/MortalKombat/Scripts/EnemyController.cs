using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MortalKombat
{
    public class EnemyController : MonoBehaviour
    {
        private GameObject enemyPlayer;
        private GameObject player1;
        private float timer = -3f; // Timer to keep track of elapsed time
        private float interval = 1f; // Time interval in seconds

        // Start is called before the first frame update
        void Start()
        {
            enemyPlayer = GameObject.Find("Player2");
            player1 = GameObject.Find("Player1");
        }

        // Update is called once per frame
        void Update()
        {
            if (enemyPlayer == null || player1 == null)
            {
                enemyPlayer = GameObject.Find("Player2");
                player1 = GameObject.Find("Player1");
            }
            timer += Time.deltaTime;

            // every 1 second, the enemy will get the current position of the player 
            // if the player is in front of the enemy, the enemy will move forward
            // if the player is behind the enemy, the enemy will move back
            // if the player is at the same position, the enemy will attack
            if (timer >= interval) // This checks approximately every second assuming 60 FPS
            {
                timer = 0f;
                TakeDecision();
            }
        }

        void TakeDecision()
        {
            var playerController = player1.GetComponent<Player1Controller>();
            var enemyController = enemyPlayer.GetComponent<Player1Controller>();
            if (enemyController.health <= 10 || playerController.health <= 10)
            {
                return;
            }

            // Reset all actions
            enemyController.forwardAuto = false;
            enemyController.backwardAuto = false;
            enemyController.primaryHitAuto = false;
            enemyController.secondaryHitAuto = false;

            // Get the current position of the player   
            float player1Position = player1.transform.position.z;
            float enemyPosition = enemyPlayer.transform.position.z;
            
            // If the player is in front of the enemy with close distance, the enemy will attack
            if (player1Position > enemyPosition - 1 && player1Position < enemyPosition + 1.5)
            {
                int rand = Random.Range(0, 2);
                if (rand == 0)
                    enemyController.secondaryHitAuto = true;
                else
                    enemyController.primaryHitAuto = true;
                timer -= 2;
            }

            // If the player is in front of the enemy, the enemy will move forward
            if (player1Position > enemyPosition + 1.4)
            {
                enemyController.forwardAuto = true;
            }

            // If the player is behind the enemy, the enemy will move back
            if (player1Position < enemyPosition - 1.5)
            {
                enemyController.backwardAuto = true;
            }

        }
    }
}
