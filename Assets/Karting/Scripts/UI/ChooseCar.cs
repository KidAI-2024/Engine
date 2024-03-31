using System;
using UnityEngine;
using UnityEngine.UI;
namespace Karting.UI
{
    public class ChooseCar : MonoBehaviour
    {
        public Button DodgeButton;
        public GameObject DodgeCarPrefab;
        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            DodgeButton.onClick.AddListener(() => gameManager.SelectCar(DodgeCarPrefab));
            DodgeButton.onClick.AddListener(() => gameManager.StartCustomMatch());
        }
    }
}
