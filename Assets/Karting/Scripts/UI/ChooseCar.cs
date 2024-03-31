using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Karting.UI
{
    public class ChooseCar : MonoBehaviour
    {
        public Button selectButton;
        public List<GameObject> carPrefabs;
        private int currentCarIndex = 0;
        public Transform carSpawnPoint;
        private GameObject instantiatedCar;

        void Start()
        {
            // Get the GameManager instance
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            // Add a listener to the select button
            // When the button is clicked, select the current car of the game manager
            selectButton.onClick.AddListener(() => SelectCurrentCar(gameManager));
            // if this is the scene before the match, start the match
            selectButton.onClick.AddListener(() => gameManager.StartCustomMatch());
            InstantiateCurrentCar();
        }

        private void SelectCurrentCar(Game.GameManager gameManager)
        {
            if (currentCarIndex < carPrefabs.Count)
            {
                GameObject selectedCarPrefab = carPrefabs[currentCarIndex];
                gameManager.SelectCar(selectedCarPrefab);
            }
            else
            {
                Debug.LogError("Invalid car index!");
            }
        }
        private void DestroyInstantiatedCar()
        {
            instantiatedCar.SetActive(false);
            Destroy(instantiatedCar);
        }
        private void InstantiateCurrentCar()
        {
            instantiatedCar = Instantiate(carPrefabs[currentCarIndex], carSpawnPoint.position, carSpawnPoint.rotation);
        }
        public void NextCar()
        {
            DestroyInstantiatedCar();
            currentCarIndex++;
            if (currentCarIndex >= carPrefabs.Count)
            {
                currentCarIndex = 0;
            }
            InstantiateCurrentCar();
        }
        public void PreviousCar()
        {
            DestroyInstantiatedCar();
            currentCarIndex--;
            if (currentCarIndex < 0)
            {
                currentCarIndex = carPrefabs.Count - 1;
            }
            InstantiateCurrentCar();
        }
        void ondestroy()
        {
            selectButton.onClick.RemoveAllListeners();

        }
    }
}
