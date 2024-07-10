using UnityEngine;
using UnityEngine.UI;
namespace GlobalAssets.HandPoseTraining
{
    public class ToggleColorChanger : MonoBehaviour
    {
        Toggle toggle;
        public Image toggleBackground;

        void Start()
        {
            // Ensure the toggle and image references are set
            if (toggle == null) toggle = GetComponent<Toggle>();
            if (toggleBackground == null) toggleBackground = GetComponent<Image>();

            // Set initial color
            UpdateColor(toggle.isOn);

            // Add listener for when the toggle state changes
            toggle.onValueChanged.AddListener(UpdateColor);
        }

        public void UpdateColor(bool isOn)
        {
            if (isOn)
            {
                toggleBackground.color = Color.white; // Change to green when on
            }
            else
            {
                toggleBackground.color = Color.red; // Change to red when off
            }
        }

        void OnDestroy()
        {
            // Remove listener when the script is destroyed to avoid memory leaks
            if (toggle != null) toggle.onValueChanged.RemoveListener(UpdateColor);
        }
    }

}