using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Karting.Game
{
    public class FinishLine : MonoBehaviour
    {
        public GameObject gamePlayManagerObj;
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player has crossed the finish line");
                // GameManager.Instance.PlayerCrossedFinishLine();
                // call the win function from gameplay manager
                gamePlayManagerObj.GetComponent<GameplayManager>().Win();
            }
        }
    }
}
