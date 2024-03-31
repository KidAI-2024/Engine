using System;
using UnityEngine;
using UnityEngine.UI;
namespace Karting.UI
{
    public class ChooseCar : MonoBehaviour
    {
        public Button DodgeButton;
        void Start()
        {
            Game.GameManager gameManager = FindObjectOfType<Game.GameManager>();
            DodgeButton.onClick.AddListener(() => gameManager.SelectCar_Dodge());
            DodgeButton.onClick.AddListener(() => gameManager.StartCustomMatch());
        }
    }
}
