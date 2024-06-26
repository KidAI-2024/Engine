using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonHoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Color originalColor;
    public Color hoverColor; // Set this color in the Inspector
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public bool isSelectable = false;
    bool isSelected = false;

    void Start()
    {
        button = GetComponent<Button>();
        audioSource = audioSource == null ? GetComponent<AudioSource>(): audioSource;
        originalColor = button.image.color;

        if (isSelectable)
        {
            button.onClick.AddListener(() =>
            {
                // get all brothers of this button
                ButtonHoverController[] brothers = transform.parent.GetComponentsInChildren<ButtonHoverController>();
                foreach (ButtonHoverController brother in brothers)
                {
                    brother.ButtonDeselected();
                    brother.isSelected = false;
                }
                ButtonSelected();
            });
        }
    }

    public void ButtonSelected()
    {
        if (button == null) {
            button = GetComponent<Button>();
        }
        button.image.color = hoverColor;
        isSelected = true;
    }
    public void ButtonDeselected()
    {
        if (button == null) {
            button = GetComponent<Button>();
        }
        button.image.color = originalColor;
        isSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button == null) {
            button = GetComponent<Button>();
        }
        button.image.color = hoverColor;
        audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button == null) {
            button = GetComponent<Button>();
        }
        if (!isSelected)
        {
            button.image.color = originalColor;
        }
    }
}
