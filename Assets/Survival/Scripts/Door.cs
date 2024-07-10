using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survival
{
    public class Door : MonoBehaviour
    {
        public GameObject handUI;
        public GameObject UIText;
        public GameObject exitText;
        public GameObject invKey;
        public GameObject fadeFX;
        public string nextSceneName; // Name of the next scene to load
        private bool inReach;
        void Start()
        {
            handUI.SetActive(false);
            UIText.SetActive(false);
            invKey.SetActive(false);
            fadeFX.SetActive(false);
            exitText.SetActive(false);
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                inReach = true;
                handUI.SetActive(true);
                // show the child object of the door
                exitText.SetActive(true);
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Reach")
            {
                inReach = false;
                handUI.SetActive(false);
                UIText.SetActive(false);
                exitText.SetActive(false);

            }
        }

        void Update()
        {

            // if (inReach && Input.GetButtonDown("Interact") && !invKey.activeInHierarchy)
            if (inReach && Input.GetButtonDown("Interact") && PlayerController.Instance.numOfCorrectClassifiedImgs < 5)
            {
                handUI.SetActive(true);
                UIText.SetActive(true);
            }

            // if (inReach && Input.GetButtonDown("Interact") && invKey.activeInHierarchy)
            if (inReach && Input.GetButtonDown("Interact") && PlayerController.Instance.numOfCorrectClassifiedImgs >= 5)
            {

                handUI.SetActive(false);
                UIText.SetActive(false);
                fadeFX.SetActive(true);
                // Get the Animator component from the target GameObject
                Animator animator = this.gameObject.GetComponent<Animator>();
                if (animator != null)
                {
                    // Enable the Animator
                    // animator.enabled = true;
                    animator.SetTrigger("StartAnimation");

                    Debug.Log("animator is activated");
                }
                else
                {
                    Debug.LogError("Animator component not found on the target GameObject.");
                }
                StartCoroutine(ending());
            }
        }

        IEnumerator ending()
        {
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene(nextSceneName);
        }

    }
}