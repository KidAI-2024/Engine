using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ButtonHoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Color originalColor;
    public Color hoverColor; // Set this color in the Inspector
    private AudioSource audioSource;
    public AudioClip hoverSound;
    bool isSelected = false;

    void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();
        originalColor = button.image.color;
    }

    public void ButtonSelected()
    {
        button.image.color = hoverColor;
        isSelected = true;
    }
    public void ButtonDeselected()
    {
        button.image.color = originalColor;
        isSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.image.color = hoverColor;
        audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            button.image.color = originalColor;
        }
    }
}
