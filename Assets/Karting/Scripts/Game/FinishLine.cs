using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karting.Game
{
    public class FinishLine : MonoBehaviour
    {
        public GameObject gamePlayManagerObj;
        public GameObject hiddenCheckpoints;
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //check that all hidden checkpoints have been reached (disabled)
                Transform[] checkpoints = hiddenCheckpoints.GetComponentsInChildren<Transform>(true);
                foreach (Transform checkpoint in checkpoints)
                {
                    if (checkpoint.CompareTag("KartingCheckpoint") && checkpoint.gameObject.activeSelf)
                    {
                        Debug.Log("Player has not reached all hidden checkpoints");
                        return;
                    }
                }
                Debug.Log("Player has crossed the finish line");
                // GameManager.Instance.PlayerCrossedFinishLine();
                // call the win function from gameplay manager
                gamePlayManagerObj.GetComponent<GameplayManager>().Win();
            }
        }
    }
}
