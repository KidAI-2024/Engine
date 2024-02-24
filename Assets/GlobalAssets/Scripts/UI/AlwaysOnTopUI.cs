using UnityEngine;

public class AlwaysOnTopUI : MonoBehaviour
{
    public Canvas canvas;
    public int sortingOrder = 10;

    void Start()
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
    }
    public void MakeOnTop()
    {
        canvas.sortingOrder = 11;
    }
    public void ResetOrder()
    {
        canvas.sortingOrder = 2;
    }
}
